using Anotar.Serilog;
using HtmlAgilityPack;
using MouseoverPopup.Interop;
using PluginManager.Interop.Sys;
using SuperMemoAssistant.Interop.SuperMemo.Elements.Builders;
using SuperMemoAssistant.Services;
using SuperMemoAssistant.Sys.Remoting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace SuperMemoAssistant.Plugins.MouseoverCompHope
{
  [Serializable]
  public class ContentService : PerpetualMarshalByRefObject, IMouseoverContentProvider
  {

    private MouseoverCompHopeCfg Config => Svc<MouseoverCompHopePlugin>.Plugin.Config;
    private string DictRegex = Svc<MouseoverCompHopePlugin>.Plugin.DictRegex;

    private readonly HttpClient _httpClient;

    public ContentService()
    {
      _httpClient = new HttpClient();
      _httpClient.DefaultRequestHeaders.Accept.Clear();
      _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public void Dispose()
    {
      _httpClient?.Dispose();
    }

    public RemoteTask<PopupContent> FetchHtml(RemoteCancellationToken ct, string href)
    {
      try
      {

        if (href.IsNullOrEmpty() || !new Regex(DictRegex).Match(href).Success)
          return null;

        return GetDictionaryEntry(ct, href);

      }
      catch (TaskCanceledException) { }
      catch (Exception ex)
      {
        LogTo.Error($"Failed to FetchHtml for href {href} with exception {ex}");
        throw;
      }

      return null;
    }

    private async Task<PopupContent> GetDictionaryEntry(RemoteCancellationToken ct, string url)
    {

      string response = await GetAsync(ct.Token(), url);
      return CreatePopupContent(response, url);

    }

    private PopupContent CreatePopupContent(string content, string url)
    {

      if (content.IsNullOrEmpty() || url.IsNullOrEmpty())
        return null;

      var doc = new HtmlDocument();
      doc.LoadHtml(content);

      doc = doc.ConvRelToAbsLinks("https://www.computerhope.com");

      var article = doc.DocumentNode.SelectSingleNode("//article");
      if (article.IsNull())
        return null;

      var titleNode = article.SelectSingleNode("//h1");

      string html = @"
          <html>
            <body>
              <p>{0}<p>
            </body>
          </html>";

      html = String.Format(html, titleNode.InnerHtml);

      var refs = new References();
      refs.Title = titleNode.InnerText;
      refs.Source = "Computer Hope";
      refs.Link = url;

      return new PopupContent(refs, html, true, browserQuery: url);
 
    }

    private async Task<string> GetAsync(CancellationToken ct, string url)
    {
      HttpResponseMessage responseMsg = null;

      try
      {
        responseMsg = await _httpClient.GetAsync(url, ct);

        if (responseMsg.IsSuccessStatusCode)
        {
          return await responseMsg.Content.ReadAsStringAsync();
        }
        else
        {
          return null;
        }
      }
      catch (HttpRequestException)
      {
        if (responseMsg != null && responseMsg.StatusCode == System.Net.HttpStatusCode.NotFound)
          return null;
        else
          throw;
      }
      catch (OperationCanceledException)
      {
        return null;
      }
      finally
      {
        responseMsg?.Dispose();
      }
    }
  }
}

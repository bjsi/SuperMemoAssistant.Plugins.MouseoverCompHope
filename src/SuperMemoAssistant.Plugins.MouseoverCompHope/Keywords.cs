using Anotar.Serilog;
using SuperMemoAssistant.Extensions;
using SuperMemoAssistant.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMemoAssistant.Plugins.MouseoverCompHope
{
  public static class Keywords
  {
    public static Dictionary<string, string> KeywordMap => CreateKeywordMap();
    public static MouseoverCompHopeCfg Config => Svc<MouseoverCompHopePlugin>.Plugin.Config;

    private static Dictionary<string, string> CreateKeywordMap()
    {
      // Copied manually to development plugins folder
      // var outPutDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);
      // TODO: Wouldn't work unless hard coded ?????????????

      var jsonPath = Path.Combine(@"C:\Users\james\SuperMemoAssistant\Plugins\Development\SuperMemoAssistant.Plugins.MouseoverCSDict\dictionary\dictionary_entries");

      try
      {

        using (StreamReader r = new StreamReader(jsonPath))
        {

          string json = r.ReadToEnd();
          var jObj = json.Deserialize<Dictionary<string, string>>();
          var adjustedJObj = new Dictionary<string, string>();

          if (!jObj.IsNull())
          {
            foreach (var entry in jObj)
            {
              var val = Config.PathToIndexHtml + entry.Value;
              adjustedJObj.Add(entry.Key, val);
            }
          }

          return adjustedJObj;

        }
      }
      catch (FileNotFoundException)
      {

        LogTo.Error($"Failed to CreateKeywordMap because {jsonPath} does not exist");
        return null;

      }
      catch (IOException e)
      {

        LogTo.Error($"Exception {e} thrown when attempting to read from {jsonPath}");
        return null;

      }
    }

  }
}

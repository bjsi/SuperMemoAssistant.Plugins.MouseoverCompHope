using Forge.Forms.Annotations;
using Newtonsoft.Json;
using SuperMemoAssistant.Services.UI.Configuration;
using SuperMemoAssistant.Sys.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMemoAssistant.Plugins.MouseoverCompHope
{
  [Form(Mode = DefaultFields.None)]
  [Title("Dictionary Settings",
       IsVisible = "{Env DialogHostContext}")]
  [DialogAction("cancel",
      "Cancel",
      IsCancel = true)]
  [DialogAction("save",
      "Save",
      IsDefault = true,
      Validates = true)]
  public class MouseoverCompHopeCfg : CfgBase<MouseoverCompHopeCfg>, INotifyPropertyChangedEx
  {
    [Title("Plugin Name")]

    [Heading("By Jamesb | Experimental Learning")]

    [Heading("Features:")]
    [Text(@"- Load dictionary definitions from Oxford Dictionary of Computer Science")]

    [Heading("General Settings")]
    [Field(Name = "Path to index.html")]
    public string PathToIndexHtml { get; set; } = @"C:\Users\james\Desktop\Oxford Dictionary Of Computer Science\index.html";

    [Heading("Keyword Scanning Settings")]

    [Heading("Reference Regexes")]
    [Field(Name = "Title Regexes")]
    [MultiLine]
    public string ReferenceTitleRegexes { get; set; } = ".*Computer Science.*";

    [Field(Name = "Author Regexes")]
    [MultiLine]
    public string ReferenceAuthorRegexes { get; set; }

    [Field(Name = "Source Regexes")]
    [MultiLine]
    public string ReferenceSourceRegexes { get; set; }

    [Field(Name = "Link Regexes")]
    public string ReferenceLinkRegexes { get; set; }

    [Field(Name = "Concept Regexes")]
    [MultiLine]
    public string ConceptNameRegexes { get; set; } = "Computer Science";

    [JsonIgnore]
    public bool IsChanged { get; set; }

    public override string ToString()
    {
      return "Mouseover Comp Hope Settings";
    }

    public event PropertyChangedEventHandler PropertyChanged;
  }
}

#region using
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Common;
using Microsoft.InformationProtection;
using Exception = System.Exception; 
#endregion

namespace Common
{
    public static class LabelHelper
    {
        private static Type T = typeof(LabelHelper);

        public static string GetLabelFullName(Label label)
        {
            if (label == null) { return null; }
            var fullName = label.Parent != null ? $"{label.Parent.Name} {label.Name.ToLower()}" : label.Name;
            return fullName;
        }

        public static string GetLabelId(Label label)
        {
            if (label == null) { return null; }
            //var fullName = label.Parent != null ? $"{label.Parent.Name.Replace(" ", "")}.{label.Name.Replace(" ", "")}" : label.Name;
            return label.Id;
        }
        public static string GetLabelFullNameTrimmed(Label label)
        {
            if (label == null) { return null; }
            var fullName = label.Parent != null ? $"{label.Parent.Name.Replace(" ", "")}.{label.Name.Replace(" ", "")}" : label.Name;
            return fullName?.Replace(" ", "").Replace("(", "").Replace(")", "");
        }

        public static string GetLabelId(string propertyName)
        {
            if (propertyName.StartsWith("MSIP_Label_"))
            {
                string match = @"(\{){0,1}[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}(\}){0,1}";
                if (Regex.IsMatch(propertyName, match)) return Regex.Match(propertyName, match).Value;
            }
            return "";
        }
        public static string GetLabelId(dynamic properties)
        {
            foreach (var prop in properties)
            {
                string Name = prop.Name;

                if (Name.StartsWith("MSIP_Label_"))
                {
                    string match = @"(\{){0,1}[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}(\}){0,1}";
                    if (Regex.IsMatch(Name, match))
                        return Regex.Match(Name, match).Value;
                }
            }
            return "";
        }

        //public static IDictionary<string, string> GetMIPMetadata(dynamic document)
        //{
        //    if (document is MailItem mailItem)
        //    {
        //        var propertyAccessor = mailItem.PropertyAccessor;
        //        var wordEditor = mailItem.GetInspector.WordEditor;
                
        //        var mipLabels = default(string);
        //        try { mipLabels = propertyAccessor.GetProperty("http://schemas.microsoft.com/mapi/string/{00020386-0000-0000-C000-000000000046}/msip_labels") as string; } catch (Exception ex) { TraceManager.Exception(ex);  }
        //        var labelAttributes = mipLabels?.Split(';')?.Select(s => s.Trim())?.Where(s => !string.IsNullOrEmpty(s));
        //        var metadataStrings = labelAttributes?.Where(s => s.StartsWith("MSIP_Label_"))?.ToList();
        //        var metadata = metadataStrings?.ToDictionary(s => s.Split('=').First(), s => s.Split('=').Last());
        //        return metadata;
        //    }
        //    else
        //    {
        //        var properties = OfficeHelper.GetCustomProperties(document);
        //        var metadata = default(Dictionary<string, string>);
        //        if (properties != null)
        //        {
        //            foreach (var prop in properties)
        //            {
        //                // TraceManager.Debug(new { prop.Name, prop.Value, prop.Type, prop.LinkToContent, prop.LinkSource });
        //                if (prop.Name.StartsWith("MSIP_Label_"))
        //                {
        //                    if (metadata == null) { metadata = new Dictionary<string, string>(); }
        //                    metadata.Add(prop.Name, prop.Value);
        //                }
        //            }
        //        }
        //        return metadata;
        //    }
        //}
        //public static string GetMIPMetadataAttribute(dynamic document, string attributeName)
        //{
        //    var attributes = GetMIPMetadata(document) as IDictionary<string, string>;
        //    var attributeValue = attributes?.FirstOrDefault(s => !string.IsNullOrEmpty(s.Key) && s.Key.EndsWith($"_{attributeName}")).Value;
        //    return attributeValue;
        //}
        //public static string GetMIPMetadataLabelId(dynamic document)
        //{
        //    var attributes = GetMIPMetadata(document) as IDictionary<string, string>;
        //    var attributeName = attributes?.FirstOrDefault().Key;
        //    var labelId = attributeName?.Substring("MSIP_Label_".Length)?.Split('_')?.FirstOrDefault();
        //    return labelId;
        //}
        //public static Label GetDocumentLabel(dynamic document, List<Label> leafLabels)
        //{
        //    using (var sec = TraceManager.GetCodeSection(T))
        //    {
        //        if (document is MailItem mailItem)
        //        {
        //            var propertyAccessor = mailItem.PropertyAccessor;
        //            var wordEditor = mailItem.GetInspector.WordEditor;

        //            var mipLabels = default(string);
        //            try { mipLabels = propertyAccessor.GetProperty("http://schemas.microsoft.com/mapi/string/{00020386-0000-0000-C000-000000000046}/msip_labels") as string; } catch (Exception ex) { TraceManager.Exception(ex); }
        //            var labelAttributes = mipLabels?.Split(';')?.Select(s => s.Trim())?.Where(s => !string.IsNullOrEmpty(s));
        //            var labelAttribute = labelAttributes?.FirstOrDefault(s => s.StartsWith("MSIP_Label_"));
        //            var labelID = !string.IsNullOrEmpty(labelAttribute) ? LabelHelper.GetLabelId(labelAttribute) : null;

        //            var documentLabel = !string.IsNullOrEmpty(labelID) ? leafLabels.FirstOrDefault(l => l.Id == labelID) : null;
        //            return documentLabel;
        //        }
        //        else
        //        {
        //            var properties = OfficeHelper.GetCustomProperties(document);
        //            dynamic msipLabelProperty = null;
        //            if (properties != null)
        //            {
        //                foreach (var prop in properties)
        //                {
        //                    sec.Debug(new { prop.Name, prop.Value, prop.Type, prop.LinkToContent, prop.LinkSource });
        //                    if (prop.Name.StartsWith("MSIP_Label_")) { msipLabelProperty = prop; break; }
        //                }
        //            }
        //            var propertyMame = msipLabelProperty != null ? msipLabelProperty.Name as string : null;
        //            var labelID = !string.IsNullOrEmpty(propertyMame) ? LabelHelper.GetLabelId(propertyMame) : null;
        //            var documentLabel = !string.IsNullOrEmpty(labelID) ? leafLabels.FirstOrDefault(l => l.Id == labelID) : null;

        //            return documentLabel;
        //        }
        //    }
        //}
    }
}

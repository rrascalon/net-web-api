﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Net.Web.Api.Sdk.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "16.3.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("2097152")]
        public long MaxAllowedUploadSize {
            get {
                return ((long)(this["MaxAllowedUploadSize"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(@"<?xml version=""1.0"" encoding=""utf-16""?>
<ArrayOfString xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
  <string>application/msword</string>
  <string>application/vnd.openxmlformats-officedocument.wordprocessingml.document</string>
  <string>application/vnd.openxmlformats-officedocument.wordprocessingml.template</string>
  <string>application/vnd.ms-excel</string>
  <string>application/vnd.openxmlformats-officedocument.spreadsheetml.sheet</string>
  <string>application/vnd.openxmlformats-officedocument.spreadsheetml.template</string>
  <string>application/vnd.ms-powerpoint</string>
  <string>application/vnd.openxmlformats-officedocument.presentationml.presentation</string>
  <string>application/vnd.openxmlformats-officedocument.presentationml.template</string>
  <string>application/vnd.openxmlformats-officedocument.presentationml.slideshow</string>
  <string>image/bmp</string>
  <string>image/gif</string>
  <string>image/jpeg</string>
  <string>image/svg+xml</string>
  <string>image/tiff</string>
  <string>image/x-icon</string>
  <string>image/png</string>
  <string>application/json</string>
  <string>application/pdf</string>
  <string>text/plain</string>
  <string>application/xml</string>
  <string>text/xml</string>
</ArrayOfString>")]
        public global::System.Collections.Specialized.StringCollection AllowedMimeTypes {
            get {
                return ((global::System.Collections.Specialized.StringCollection)(this["AllowedMimeTypes"]));
            }
        }
    }
}

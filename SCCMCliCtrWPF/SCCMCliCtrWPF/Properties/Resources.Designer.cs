﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ClientCenter.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("ClientCenter.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized resource of type System.Drawing.Icon similar to (Icon).
        /// </summary>
        internal static System.Drawing.Icon clictr {
            get {
                object obj = ResourceManager.GetObject("clictr", resourceCulture);
                return ((System.Drawing.Icon)(obj));
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;ActionDescription Class=&quot;Executable&quot; SelectionMode=&quot;Both&quot; DisplayName=&quot;Client Center&quot; MnemonicDisplayName=&quot;Client Center...&quot; Description=&quot;Open Client Center...&quot;&gt;
        ///	&lt;ShowOn&gt;
        ///		&lt;string&gt;ContextMenu&lt;/string&gt;
        ///	&lt;/ShowOn&gt;
        ///	&lt;ResourceAssembly&gt;
        ///		&lt;Assembly&gt;{0}&lt;/Assembly&gt;
        ///		&lt;Type&gt;ClientCenter.Properties.Resources.resources&lt;/Type&gt;
        ///	&lt;/ResourceAssembly&gt;
        ///	&lt;ImagesDescription&gt;
        ///		&lt;ResourceAssembly&gt;
        ///			&lt;Assembly&gt;{0}&lt;/Assembly&gt;
        ///			&lt;Type&gt;ClientCenter.Properties.Resources.resources&lt;/Type&gt;
        ///		&lt;/ResourceAssembly&gt;
        ///		&lt;Im [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string ConsoleExtension {
            get {
                return ResourceManager.GetString("ConsoleExtension", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This license governs use of the accompanying software. If you use the software, you
        /// accept this license. If you do not accept the license, do not use the software.
        ///
        ///1. Definitions
        /// The terms &quot;reproduce,&quot; &quot;reproduction,&quot; &quot;derivative works,&quot; and &quot;distribution&quot; have the
        /// same meaning here as under U.S. copyright law.
        /// A &quot;contribution&quot; is the original software, or any additions or changes to the software.
        /// A &quot;contributor&quot; is any person that distributes its contribution under this license.
        /// &quot;Licensed pa [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string License {
            get {
                return ResourceManager.GetString("License", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to $script = {
        ///function Enable-TSDuplicateToken {
        ///&lt;#
        ///  .SYNOPSIS
        ///  Duplicates the Access token of lsass and sets it in the current process thread.
        ///
        ///  .DESCRIPTION
        ///  The Enable-TSDuplicateToken CmdLet duplicates the Access token of lsass and sets it in the current process thread.
        ///  The CmdLet must be run with elevated permissions.
        ///
        ///  .EXAMPLE
        ///  Enable-TSDuplicateToken
        ///
        ///  .LINK
        ///  http://www.truesec.com
        ///
        ///  .NOTES
        ///  Goude 2012, TreuSec
        ///#&gt;
        ///[CmdletBinding()]
        ///param()
        ///
        ///$signature = @&quot;
        ///    [Struc [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string PSCollDecode {
            get {
                return ResourceManager.GetString("PSCollDecode", resourceCulture);
            }
        }
    }
}

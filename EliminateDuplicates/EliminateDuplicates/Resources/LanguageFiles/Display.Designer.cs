﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DeleteDuplicateFiles.Resources.LanguageFiles {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class Display {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Display() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("DeleteDuplicateFiles.Resources.LanguageFiles.Display", typeof(Display).Assembly);
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
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Are you sure you want to cancel the search?.
        /// </summary>
        public static string CancelSearch_Confirm_Question {
            get {
                return ResourceManager.GetString("CancelSearch_Confirm_Question", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Conflicting item detected, folder was not added.
        /// </summary>
        public static string frmMain_AddSearshDirectory_conflicting_item {
            get {
                return ResourceManager.GetString("frmMain_AddSearshDirectory_conflicting_item", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Are you sure you want to clear all directories from list?.
        /// </summary>
        public static string frmMain_clarAlSearchDirConfirmationQuesion {
            get {
                return ResourceManager.GetString("frmMain_clarAlSearchDirConfirmationQuesion", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Are you sure you want to clear all search results?.
        /// </summary>
        public static string frmMain_ClearSearchFolders_ConfirmationRequest {
            get {
                return ResourceManager.GetString("frmMain_ClearSearchFolders_ConfirmationRequest", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Failed to load search profile, perhaps its and older format version?.
        /// </summary>
        public static string FrmMain_openLastProfileMenuItem_Click_Failed_to_load_search_profile {
            get {
                return ResourceManager.GetString("FrmMain_openLastProfileMenuItem_Click_Failed_to_load_search_profile", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Hash database file is still loading, please try again in 5 seconds..
        /// </summary>
        public static string frmMain_SearchForDuplicateFiles_ComputeHashServUnavailable {
            get {
                return ResourceManager.GetString("frmMain_SearchForDuplicateFiles_ComputeHashServUnavailable", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Hash database is loading.
        /// </summary>
        public static string frmMain_SearchForDuplicateFiles_DatabaseIsLoading {
            get {
                return ResourceManager.GetString("frmMain_SearchForDuplicateFiles_DatabaseIsLoading", resourceCulture);
            }
        }
    }
}

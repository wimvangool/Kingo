﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18047
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace YellowFlare.MessageProcessing.Resources {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class ExceptionMessages {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal ExceptionMessages() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("YellowFlare.MessageProcessing.Resources.ExceptionMessages", typeof(ExceptionMessages).Assembly);
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
        ///   Looks up a localized string similar to The clock-scopes aren&apos;t nested correctly..
        /// </summary>
        internal static string ClockScope_IncorrectNesting {
            get {
                return ResourceManager.GetString("ClockScope_IncorrectNesting", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Command &apos;{0}&apos; failed because it caused a DomainException to occur..
        /// </summary>
        internal static string DomainException_CommandFailed {
            get {
                return ResourceManager.GetString("DomainException_CommandFailed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The controller failed to enlist the specified UnitOfWork because the MessageProcessor is not handling any message on the current Thread..
        /// </summary>
        internal static string MessageProcessor_FailedToEnlistUnitOfWork {
            get {
                return ResourceManager.GetString("MessageProcessor_FailedToEnlistUnitOfWork", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The scope has already completed..
        /// </summary>
        internal static string UnitOfWorkContext_ScopeAlreadyCompleted {
            get {
                return ResourceManager.GetString("UnitOfWorkContext_ScopeAlreadyCompleted", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Invalid type specified for comparison. Expected &apos;{0}&apos; but was &apos;{1}&apos;..
        /// </summary>
        internal static string Version_InvalidType {
            get {
                return ResourceManager.GetString("Version_InvalidType", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Version cannot be negative: {0}..
        /// </summary>
        internal static string Version_NegativeValue {
            get {
                return ResourceManager.GetString("Version_NegativeValue", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No event-handler was specified for domain-event of type &apos;{0}&apos;..
        /// </summary>
        internal static string WritableEventStream_MissingEventHandler {
            get {
                return ResourceManager.GetString("WritableEventStream_MissingEventHandler", resourceCulture);
            }
        }
    }
}

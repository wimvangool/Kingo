﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34209
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Kingo.Resources {
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
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Kingo.Resources.ExceptionMessages", typeof(ExceptionMessages).Assembly);
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
        ///   Looks up a localized string similar to Cannot pick a value of type &apos;{0}&apos; because the specified collection is empty..
        /// </summary>
        internal static string Scenario_EmptyCollectionSpecified {
            get {
                return ResourceManager.GetString("Scenario_EmptyCollectionSpecified", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot set an expectation for event on index {0} because an expectation has alreayd been set for this event..
        /// </summary>
        internal static string Scenario_EventExpectationAlreadySet {
            get {
                return ResourceManager.GetString("Scenario_EventExpectationAlreadySet", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot set the expectation for exception of type &apos;{0}&apos; because another expectation has already been set..
        /// </summary>
        internal static string Scenario_ExceptionExpectationAlreadySet {
            get {
                return ResourceManager.GetString("Scenario_ExceptionExpectationAlreadySet", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Expected exception was not thrown..
        /// </summary>
        internal static string Scenario_ExpectedExceptionNotThrown {
            get {
                return ResourceManager.GetString("Scenario_ExpectedExceptionNotThrown", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Invalid event count specified: {0}. Number must be 0 or higher..
        /// </summary>
        internal static string Scenario_InvalidEventCount {
            get {
                return ResourceManager.GetString("Scenario_InvalidEventCount", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Invalid mode has been set for scenario &apos;{0}&apos;: {1}..
        /// </summary>
        internal static string Scenario_InvalidMode {
            get {
                return ResourceManager.GetString("Scenario_InvalidMode", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Index cannot be negative: {0}..
        /// </summary>
        internal static string Scenario_NegativeIndex {
            get {
                return ResourceManager.GetString("Scenario_NegativeIndex", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unexpected amount of events were published. Expected: {0}, actual: {1}..
        /// </summary>
        internal static string Scenario_UnexpectedEventCount {
            get {
                return ResourceManager.GetString("Scenario_UnexpectedEventCount", resourceCulture);
            }
        }
    }
}

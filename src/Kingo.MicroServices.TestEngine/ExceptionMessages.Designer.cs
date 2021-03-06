﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Kingo {
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
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Kingo.ExceptionMessages", typeof(ExceptionMessages).Assembly);
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
        ///   Looks up a localized string similar to Event of type &apos;{0}&apos; not found at index &apos;{1}&apos;: the EventStream contains only {2} event(s)..
        /// </summary>
        internal static string EventStream_EventNotFound {
            get {
                return ResourceManager.GetString("EventStream_EventNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Expected event of type &apos;{0}&apos; at index &apos;{1}&apos;, but found event of type &apos;{2}&apos;..
        /// </summary>
        internal static string EventStream_EventNotOfExpectedType {
            get {
                return ResourceManager.GetString("EventStream_EventNotOfExpectedType", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to One or more assertions for the published events failed. See inner exception for details. .
        /// </summary>
        internal static string MessageHandlerResult_AssertionOfEventStreamFailed {
            get {
                return ResourceManager.GetString("MessageHandlerResult_AssertionOfEventStreamFailed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Expected an empty stream, but stream contains {0} event(s)..
        /// </summary>
        internal static string MessageHandlerResult_StreamNotEmpty {
            get {
                return ResourceManager.GetString("MessageHandlerResult_StreamNotEmpty", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot invoke &apos;{0}&apos; at this point: the processor has already been configured..
        /// </summary>
        internal static string MicroProcessorConfiguration_ProcessorAlreadyConfigured {
            get {
                return ResourceManager.GetString("MicroProcessorConfiguration_ProcessorAlreadyConfigured", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot invoke &apos;{0}&apos; at this point: the processor has not yet been configured..
        /// </summary>
        internal static string MicroProcessorConfiguration_ProcessorNotYetConfigured {
            get {
                return ResourceManager.GetString("MicroProcessorConfiguration_ProcessorNotYetConfigured", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot invoke &apos;{0}&apos; at this point: the service collection has already been configured..
        /// </summary>
        internal static string MicroProcessorConfiguration_ServicesAlreadyConfigured {
            get {
                return ResourceManager.GetString("MicroProcessorConfiguration_ServicesAlreadyConfigured", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot retrieve the EventStream of &apos;{0}&apos; because its results haven&apos;t been saved in this context..
        /// </summary>
        internal static string MicroProcessorTestContext_EventStreamNotFound {
            get {
                return ResourceManager.GetString("MicroProcessorTestContext_EventStreamNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot save the EventStream of &apos;{0}&apos; because a previous result of this test has already been saved..
        /// </summary>
        internal static string MicroProcessorTestContext_TestAlreadyRun {
            get {
                return ResourceManager.GetString("MicroProcessorTestContext_TestAlreadyRun", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to One or more assertions for exception of type &apos;{0}&apos; failed. See inner exception for details..
        /// </summary>
        internal static string MicroProcessorTestResult_AssertionOfExceptionFailed {
            get {
                return ResourceManager.GetString("MicroProcessorTestResult_AssertionOfExceptionFailed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Expected an exception of type &apos;{0}&apos;, but encountered an exception of type &apos;{1}&apos; instead..
        /// </summary>
        internal static string MicroProcessorTestResult_ExceptionNotOfExpectedType {
            get {
                return ResourceManager.GetString("MicroProcessorTestResult_ExceptionNotOfExpectedType", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The expected exception of type &apos;{0}&apos; was not thrown..
        /// </summary>
        internal static string MicroProcessorTestResult_ExceptionNotThrown {
            get {
                return ResourceManager.GetString("MicroProcessorTestResult_ExceptionNotThrown", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to An unexpected exception of type &apos;{0}&apos; was thrown..
        /// </summary>
        internal static string MicroProcessorTestResult_ExceptionThrown {
            get {
                return ResourceManager.GetString("MicroProcessorTestResult_ExceptionThrown", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Exception of type &apos;{0}&apos; was expected to have an inner-exception of type &apos;{1}&apos;, but did not have any inner-exception..
        /// </summary>
        internal static string MicroProcessorTestResult_InnerExceptionNull {
            get {
                return ResourceManager.GetString("MicroProcessorTestResult_InnerExceptionNull", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Test failed by default because its result was not verified..
        /// </summary>
        internal static string MicroProcessorTestResult_ResultNotVerified {
            get {
                return ResourceManager.GetString("MicroProcessorTestResult_ResultNotVerified", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Test &apos;{0}&apos; did not produce any result. Please verify that the processor provided as an argument to the WhenAsync-method has been used to handle a message or execute a query..
        /// </summary>
        internal static string NullTestResult_MissingResult {
            get {
                return ResourceManager.GetString("NullTestResult_MissingResult", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The specified number of expected errors ({0}) is invalid. This number must be greater than or equal to 1..
        /// </summary>
        internal static string RequestMessageTestBase_InvalidErrorCount {
            get {
                return ResourceManager.GetString("RequestMessageTestBase_InvalidErrorCount", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The specified message is not valid: {0}..
        /// </summary>
        internal static string RequestMessageTestBase_MessageNotValid {
            get {
                return ResourceManager.GetString("RequestMessageTestBase_MessageNotValid", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to An instance error was expected but the result of the validation contains no instance error..
        /// </summary>
        internal static string RequestMessageTestBase_NoInstanceError {
            get {
                return ResourceManager.GetString("RequestMessageTestBase_NoInstanceError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to An error for member &apos;{0}&apos; was expected, but the result contains no error for this member..
        /// </summary>
        internal static string RequestMessageTestBase_NoMemberError {
            get {
                return ResourceManager.GetString("RequestMessageTestBase_NoMemberError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The number of expected validation errors ({0}) does not match the actual amount of validation errors ({1})..
        /// </summary>
        internal static string RequestMessageTestBase_UnexpectedErrorCount {
            get {
                return ResourceManager.GetString("RequestMessageTestBase_UnexpectedErrorCount", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The expected instance error ({0}) does not match the actual instance error ({1}) based on the type of comparison specified ({2})..
        /// </summary>
        internal static string RequestMessageTestBase_UnexpectedInstanceError {
            get {
                return ResourceManager.GetString("RequestMessageTestBase_UnexpectedInstanceError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The expected error for member &apos;{0}&apos; ({1}) does not match the actual error ({2}) based on the type of comparison specified ({3})..
        /// </summary>
        internal static string RequestMessageTestBase_UnexpectedMemberError {
            get {
                return ResourceManager.GetString("RequestMessageTestBase_UnexpectedMemberError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Test &apos;{0}&apos; threw unexpected exception of type &apos;{1}&apos;. See inner exception for details..
        /// </summary>
        internal static string UnexpectedExceptionResult_UnexpectedException {
            get {
                return ResourceManager.GetString("UnexpectedExceptionResult_UnexpectedException", resourceCulture);
            }
        }
    }
}

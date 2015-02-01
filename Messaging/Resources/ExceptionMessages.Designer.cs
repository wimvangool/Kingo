﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18444
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace System.ComponentModel.Resources {
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
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("System.ComponentModel.Resources.ExceptionMessages", typeof(ExceptionMessages).Assembly);
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
        ///   Looks up a localized string similar to No aggregate of type &apos;{0}&apos; with the specified key ({1}) was found..
        /// </summary>
        internal static string AggregateNotFoundException_Message {
            get {
                return ResourceManager.GetString("AggregateNotFoundException_Message", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Invalid type specified for comparison. Expected &apos;{0}&apos; but was &apos;{1}&apos;..
        /// </summary>
        internal static string AggregateVersion_InvalidType {
            get {
                return ResourceManager.GetString("AggregateVersion_InvalidType", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot execute this task because it was already executed or canceled..
        /// </summary>
        internal static string AsyncExecutionTask_TaskAlreadyStarted {
            get {
                return ResourceManager.GetString("AsyncExecutionTask_TaskAlreadyStarted", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot execute this method because the current SynchronizationContext is null..
        /// </summary>
        internal static string AsyncObject_ContextNotSet {
            get {
                return ResourceManager.GetString("AsyncObject_ContextNotSet", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot publish event of type &apos;{0}&apos; because no bus is currently available. Make sure all publishing takes place inside a UnitOfWorkScope..
        /// </summary>
        internal static string BufferedEventBus_NoBusAvailable {
            get {
                return ResourceManager.GetString("BufferedEventBus_NoBusAvailable", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot perform the operation because the cache is currently not available..
        /// </summary>
        internal static string CacheRelay_NotAvailable {
            get {
                return ResourceManager.GetString("CacheRelay_NotAvailable", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The connection is already closed..
        /// </summary>
        internal static string Connection_AlreadyClosed {
            get {
                return ResourceManager.GetString("Connection_AlreadyClosed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The connection is already open..
        /// </summary>
        internal static string Connection_AlreadyOpen {
            get {
                return ResourceManager.GetString("Connection_AlreadyOpen", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to More than one type found that could be used as an implementation of dependency &apos;{0}&apos;: {1}..
        /// </summary>
        internal static string DependencyClass_AmbigiousMatch {
            get {
                return ResourceManager.GetString("DependencyClass_AmbigiousMatch", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Command &apos;{0}&apos; failed..
        /// </summary>
        internal static string DomainModelException_CommandFailed {
            get {
                return ResourceManager.GetString("DomainModelException_CommandFailed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A handler for event &apos;{0}&apos; has already been registered..
        /// </summary>
        internal static string EventSourcedAggregate_HandlerAlreadyRegistered {
            get {
                return ResourceManager.GetString("EventSourcedAggregate_HandlerAlreadyRegistered", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Version cannot be negative: {0}..
        /// </summary>
        internal static string IntXXVersion_NegativeValue {
            get {
                return ResourceManager.GetString("IntXXVersion_NegativeValue", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unknown value of type &apos;{0}&apos; specified: {1}..
        /// </summary>
        internal static string Message_InvalidOption {
            get {
                return ResourceManager.GetString("Message_InvalidOption", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot mark message &apos;{0}&apos; as changed because it is marked readonly..
        /// </summary>
        internal static string Message_IsReadOnly {
            get {
                return ResourceManager.GetString("Message_IsReadOnly", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A service of type &apos;{0}&apos; was already registered..
        /// </summary>
        internal static string Message_ServiceAlreadyRegistered {
            get {
                return ResourceManager.GetString("Message_ServiceAlreadyRegistered", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Invalid InstanceLifetime was specified on class &apos;{0}&apos;: {1}. .
        /// </summary>
        internal static string MessageHandlerClass_InvalidInstanceLifetimeMode {
            get {
                return ResourceManager.GetString("MessageHandlerClass_InvalidInstanceLifetimeMode", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The message is invalid. See ErrorTree for details..
        /// </summary>
        internal static string MessageProcessor_InvalidMessage {
            get {
                return ResourceManager.GetString("MessageProcessor_InvalidMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot change this collection because it is read-only..
        /// </summary>
        internal static string ReadOnlyDictionary_NotSupported {
            get {
                return ResourceManager.GetString("ReadOnlyDictionary_NotSupported", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Invalid value for StringConstraint specified: {0}..
        /// </summary>
        internal static string RequiredAttribute_InvalidStringConstraint {
            get {
                return ResourceManager.GetString("RequiredAttribute_InvalidStringConstraint", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The scopes were not nested correctly..
        /// </summary>
        internal static string Scope_IncorrectNesting {
            get {
                return ResourceManager.GetString("Scope_IncorrectNesting", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot complete this scope because it is not the current scope..
        /// </summary>
        internal static string TransactionScope_CannotCompleteScope {
            get {
                return ResourceManager.GetString("TransactionScope_CannotCompleteScope", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The scope has already completed..
        /// </summary>
        internal static string TransactionScope_ScopeAlreadyCompleted {
            get {
                return ResourceManager.GetString("TransactionScope_ScopeAlreadyCompleted", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Failed to enlist &apos;{0}&apos; because no context was found..
        /// </summary>
        internal static string UnitOfWorkContext_FailedToEnlistUnitOfWork {
            get {
                return ResourceManager.GetString("UnitOfWorkContext_FailedToEnlistUnitOfWork", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The scope was incorrectly nested or was called from a different thread than it was created on..
        /// </summary>
        internal static string UnitOfWorkScope_IncorrectNestingOrWrongThread {
            get {
                return ResourceManager.GetString("UnitOfWorkScope_IncorrectNestingOrWrongThread", resourceCulture);
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

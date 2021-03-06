﻿namespace devdeer.DoctorFlox.Ui.WpfSample.ViewModels
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;

    using Autofac;

    using Helpers;

    using Logic.Wpf;
    using Logic.Wpf.Commands;
    using Logic.Wpf.Enumerations;
    using Logic.Wpf.Helpers;
    using Logic.Wpf.Interfaces;
    using Logic.Wpf.Messages;

    /// <summary>
    /// View model for the main window.
    /// </summary>
    public class MainViewModel : BaseViewModel
    {
        #region constructors and destructors
       
        /// <inheritdoc />
        public MainViewModel(IMessenger messenger, SynchronizationContext synchronizationContext) : base(messenger, synchronizationContext)
        {
        }

        #endregion

        #region methods

        /// <inheritdoc />
        protected override void AfterInitialization()
        {
            base.AfterInitialization();
            Task.Delay(2000).ContinueWith(
                t =>
                {
                    Trace.TraceInformation($"Sending message from thread {Thread.CurrentThread.ManagedThreadId}");
                    MessengerInstance.Send(new DataMessage<MainViewModel, MainViewModel, string>("Hello"));
                });
        }

        /// <inheritdoc />
        protected override void InitCommands()
        {
            base.InitCommands();
            ShowMessageCommand = new RelayCommand(() => MessageBox.Show($"You said: {TestMessage}"), () => !string.IsNullOrEmpty(TestMessage));
            OpenChildWindowCommand = new RelayCommand(
                () =>
                {
                    var windowInstance = CreateWindowInstance("ChildWindow");
                    windowInstance?.ShowDialog();
                });
        }
        
        /// <inheritdoc />
        protected override void InitData()
        {
            base.InitData();
            Caption = "WpfSample (Runtime)";
        }

        /// <inheritdoc />
        protected override void InitDesignTimeData()
        {
            base.InitDesignTimeData();
            Caption = "WpfSample (Design)";
        }

        /// <inheritdoc />
        protected override void InitMessenger()
        {
            base.InitMessenger();            
            MessengerInstance.Register<DataMessage<MainViewModel, MainViewModel, string>>(
                this,
                ThreadCallbackOption.UiThread,
                m =>
                {
                    Trace.TraceInformation($"Receiving message on thread {Thread.CurrentThread.ManagedThreadId}");
                    Message = m.Data;
                });
        }

        #endregion

        #region properties

        /// <summary>
        /// The caption for the view.
        /// </summary>
        public string Caption { get; private set; }

        /// <summary>
        /// The last message arrived.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Triggers the opening of a new child window.
        /// </summary>
        public RelayCommand OpenChildWindowCommand { get; private set; }

        /// <summary>
        /// Can be used to show a message box.
        /// </summary>
        public RelayCommand ShowMessageCommand { get; private set; }

        /// <summary>
        /// Some test text from the UI.
        /// </summary>
        public string TestMessage { get; set; }

        /// <inheritdoc />
        protected override Func<Type, Window> WindowInstanceResolver
        {
            get
            {
                return viewType => Variables.AutoFacContainer.Resolve(viewType) as Window;
            }
        }

        #endregion
    }
}
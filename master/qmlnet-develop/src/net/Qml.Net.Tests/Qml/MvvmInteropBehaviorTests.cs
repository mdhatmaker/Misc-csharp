using System.ComponentModel;
using System.Runtime.CompilerServices;
using FluentAssertions;
using Xunit;

namespace Qml.Net.Tests.Qml
{
    public class ViewModelContainer
    {
        public ViewModel ViewModel { get; set; } = new ViewModel();

        public void ChangeStringPropertyTo(string newValue)
        {
            ViewModel.StringProperty = newValue;
        }

        public void ChangeIntPropertyTo(int newValue)
        {
            ViewModel.IntProperty = newValue;
        }

        public void ChangeCustomIntPropertyTo(int newValue)
        {
            ViewModel.CustomIntProperty = newValue;
        }

        public void ChangeCustomMvvmStyleIntPropertyTo(int newValue)
        {
            ViewModel.CustomMvvmStyleIntProperty = newValue;
        }

        public void ChangeNotifyOnlyIntPropertyTo(int newValue)
        {
            ViewModel.NotifyOnlyIntProperty = newValue;
        }

        public void ChangeCustomNotifyOnlyIntPropertyTo(int newValue)
        {
            ViewModel.CustomNotifyOnlyIntProperty = newValue;
        }

        public bool? TestResult { get; set; } = null;
    }

    public class ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string _stringProperty = string.Empty;

        public string StringProperty
        {
            get
            {
                return _stringProperty;
            }

            set
            {
                if (!Equals(value, _stringProperty))
                {
                    _stringProperty = value;
                    FirePropertyChanged();
                }
            }
        }

        private int _intProperty;

        public int IntProperty
        {
            get => _intProperty;
            set
            {
                if (!Equals(value, _intProperty))
                {
                    _intProperty = value;
                    FirePropertyChanged();
                }
            }
        }

        private int _customIntProperty;

        [NotifySignal("customIntPropertyChangedSignal")]
        public int CustomIntProperty
        {
            get
            {
                return _customIntProperty;
            }

            set
            {
                if (!Equals(value, _customIntProperty))
                {
                    _customIntProperty = value;
                    FirePropertyChanged();
                }
            }
        }

        private int _customMvvmStyleIntProperty;

        [NotifySignal]
        public int CustomMvvmStyleIntProperty
        {
            get
            {
                return _customMvvmStyleIntProperty;
            }

            set
            {
                if (!Equals(value, _customMvvmStyleIntProperty))
                {
                    _customMvvmStyleIntProperty = value;
                    FirePropertyChanged();
                }
            }
        }

        private int _notifyOnlyIntProperty;

        [NotifySignal]
        public int NotifyOnlyIntProperty
        {
            get
            {
                return _notifyOnlyIntProperty;
            }

            set
            {
                if (!Equals(value, _notifyOnlyIntProperty))
                {
                    _notifyOnlyIntProperty = value;
                    this.ActivateNotifySignal();
                }
            }
        }

        private int _customNotifyOnlyIntProperty;

        [NotifySignal("customNotifyIntPropertyChangedSignal")]
        public int CustomNotifyOnlyIntProperty
        {
            get
            {
                return _customNotifyOnlyIntProperty;
            }

            set
            {
                if (!Equals(value, _customNotifyOnlyIntProperty))
                {
                    _customNotifyOnlyIntProperty = value;
                    this.ActivateNotifySignal();
                }
            }
        }

        private void FirePropertyChanged([CallerMemberName]string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class MvvmInteropBehaviorTests : BaseQmlMvvmTestsWithInstance<ViewModelContainer>
    {
        [Fact]
        public void Does_register_property_changed_signal()
        {
            RunQmlTest(
                "viewModelContainer",
                @"
                    var vm = viewModelContainer.viewModel
                    vm.stringPropertyChanged.connect(function() {
                        viewModelContainer.testResult = true
                    })
                    viewModelContainer.changeStringPropertyTo('new value')
                ");

            Instance.TestResult.Should().Be(true);
        }

        [Fact(Skip = "This test stopped working in 5.12. TODO: investigate")]
        public void Does_unregister_signal_on_ref_destroy()
        {
            RunQmlTest("test",
                @"
                    var vm = test.viewModel;
                    vm.stringPropertyChanged.connect(function() {
                        test.testResult = false
                    });
                    vm = null;

                    gc();

                    Qt.callLater(function() {
                        test.testResult = true;
                        test.changeStringPropertyTo('new value')
                    })
                ", true);

            Instance.TestResult.Should().Be(true);
        }

        [Fact]
        public void Does_play_nicely_with_completely_custom_notify_signals()
        {
            RunQmlTest(
                "viewModelContainer",
                @"
                    var vm = viewModelContainer.viewModel
                    vm.customIntPropertyChangedSignal.connect(function() {
                        viewModelContainer.testResult = true
                    })
                    viewModelContainer.changeCustomIntPropertyTo(3)
                ");

            Instance.TestResult.Should().Be(true);
        }

        [Fact]
        public void Does_play_nicely_with_custom_notify_signals()
        {
            RunQmlTest(
                "viewModelContainer",
                @"
                    var vm = viewModelContainer.viewModel
                    vm.customMvvmStyleIntPropertyChanged.connect(function() {
                        viewModelContainer.testResult = true
                    })
                    viewModelContainer.changeCustomMvvmStyleIntPropertyTo(3)
                ");

            Instance.TestResult.Should().Be(true);
        }

        [Fact]
        public void Does_not_interfer_with_properties_only_using_notify_signals()
        {
            RunQmlTest(
                "viewModelContainer",
                @"
                    var vm = viewModelContainer.viewModel
                    vm.notifyOnlyIntPropertyChanged.connect(function() {
                        viewModelContainer.testResult = true
                    })
                    viewModelContainer.changeNotifyOnlyIntPropertyTo(3)
                ");

            Instance.TestResult.Should().Be(true);
        }

        [Fact]
        public void Does_not_interfer_with_properties_only_using_custom_notify_signals()
        {
            RunQmlTest(
                "viewModelContainer",
                @"
                    var vm = viewModelContainer.viewModel
                    vm.customNotifyIntPropertyChangedSignal.connect(function() {
                        viewModelContainer.testResult = true
                    })
                    viewModelContainer.changeCustomNotifyOnlyIntPropertyTo(3)
                ");

            Instance.TestResult.Should().Be(true);
        }
    }
}

using flow_desktop.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.Json;
using Windows.Foundation;
using Windows.Foundation.Collections;
using ABI.Microsoft.UI;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

// TODO add debounce to search
namespace flow_desktop.UI.Components
{
    // TODO add play next, play later
    //  add local listen count db
    public sealed partial class SearchBar : UserControl
    {
        private ObservableCollection<SongSearchItem>? _songSearchResults;
        
        private static readonly DependencyProperty dependencyPropOnSearch =
            DependencyProperty.Register(
                nameof(OnSearch),
                typeof(Action<string>),
                typeof(SearchBar),
                new PropertyMetadata(null)
            );

        public Action<string> OnSearch
        {
            get => (Action<string>)GetValue(dependencyPropOnSearch);
            set => SetValue(dependencyPropOnSearch, value);
        }

        private static readonly DependencyProperty dependencyPropOnPlaySong =
            DependencyProperty.Register(
                nameof(OnPlaySong),
                typeof(Action<SongSearchItem>),
                typeof(SearchBar),
                new PropertyMetadata(null)
            );

        public Action<SongSearchItem> OnPlaySong
        {
            get => (Action<SongSearchItem>)GetValue(dependencyPropOnPlaySong);
            set => SetValue(dependencyPropOnPlaySong, value);
        }
        
        private static readonly DependencyProperty dependencyPropOnPlayNext =
            DependencyProperty.Register(
                nameof(OnPlayNext),
                typeof(Action<SongSearchItem>),
                typeof(SearchBar),
                new PropertyMetadata(null)
            );

        public Action<SongSearchItem> OnPlayNext
        {
            get => (Action<SongSearchItem>)GetValue(dependencyPropOnPlayNext);
            set => SetValue(dependencyPropOnPlayNext, value);
        }
        
        private void PlaySongNext(object? sender, EventArgs e)
        {
            var songSearchSLI = (FrameworkElement)sender!;
            var song = (SongSearchItem)songSearchSLI.DataContext;
            _songSearchResults?.Remove(song);
            OnPlayNext?.Invoke(song);
        }
        

        private static readonly DependencyProperty dependencyPropOnInclude =
            DependencyProperty.Register(
                nameof(OnInclude),
                typeof(Action<SongSearchItem>),
                typeof(SearchBar),
                new PropertyMetadata(null)
            );

        public Action<SongSearchItem> OnInclude
        {
            get => (Action<SongSearchItem>)GetValue(dependencyPropOnInclude);
            set => SetValue(dependencyPropOnInclude, value);
        }

        private void IncludeSong(object? sender, EventArgs e)
        {
            var songSearchSLI = (FrameworkElement)sender!;
            var song = (SongSearchItem)songSearchSLI.DataContext;
            _songSearchResults?.Remove(song);
            OnInclude?.Invoke(song);
        }
        
        private DispatcherTimer? _debounceTimer;
        private void OnQueryChange(object sender, TextChangedEventArgs e)
        {
            var query = ((TextBox)sender).Text;
            
            _debounceTimer?.Stop();
            _debounceTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(300)
            };
            _debounceTimer.Tick += (s, args) =>
            {
                _debounceTimer.Stop();
                OnSearch?.Invoke(query);
            };
            _debounceTimer.Start();
        }
        
        private static readonly DependencyProperty dependencyPropSongSearchState =
            DependencyProperty.Register(
                nameof(dependencyWrapperSongSearchState),
                typeof(SongSearchState),
                typeof(SearchBar),
                new PropertyMetadata(
                    null, 
                    OnSearchStateChanged
                )
            );

        public SongSearchState dependencyWrapperSongSearchState
        {
            get => (SongSearchState)GetValue(dependencyPropSongSearchState);
            set => SetValue(dependencyPropSongSearchState, value);
        }
        
        private static void OnSearchStateChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e 
        )
        {
            var searchBar = (SearchBar)d;
            searchBar.UpdateSearchStateVisuals();
        }

        private void UpdateSearchStateVisuals()
        {
            SongSearchResultsPopup.IsOpen = dependencyWrapperSongSearchState is not SongSearchState.Idle;

            switch (dependencyWrapperSongSearchState)
            {
                case SongSearchState.Searching:
                    SongSearchPopupContent.Content = new ProgressRing
                    {
                        IsActive = true,
                        Foreground = (Brush)Application.Current.Resources["BrushIsco"],
                    };
                    break;
                case SongSearchState.NoResults:
                    SongSearchPopupContent.Content = new TextBlock
                    {
                        Text = "couldn't find it.",
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                    };
                    break;
                case SongSearchState.FoundResults found:
                    var listView = CreateSongSearchResultsListView();
                    _songSearchResults = new ObservableCollection<SongSearchItem>(found.SongSearchResults);
                    listView.ItemsSource = _songSearchResults;
                    SongSearchPopupContent.Content = listView;
                    break;
                case SongSearchState.Error:
                    // TODO snackbar instead...
                    SongSearchPopupContent.Content = new TextBlock
                    {
                        Text = "sumn' wrong, i hold my head.",
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                    };
                    break;
                default:
                    SongSearchPopupContent.Content = null;
                    break;
            }
        }

        private ListView CreateSongSearchResultsListView()
        {
            var listView = new ListView
            {
                Padding = new Thickness(0, 0, 0, 8),
                ItemTemplate = (DataTemplate)Resources["SongSearchSLITemplate"],
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
            };

            var itemStyle = new Style(typeof(ListViewItem));
            itemStyle.Setters.Add(new Setter(MarginProperty, new Thickness(8, 0, 0, 8)));
            itemStyle.Setters.Add(new Setter(HorizontalContentAlignmentProperty, HorizontalAlignment.Stretch));
            listView.ItemContainerStyle = itemStyle;
            
            listView.DoubleTapped += OnSongDoubleTapped;

            return listView;
        }
        
        /// <summary>
        /// double-tapping a result plays it, and closes the results.
        /// </summary>
        private void OnSongDoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            var tappedElement = e.OriginalSource as FrameworkElement;
            var song = tappedElement?.DataContext as SongSearchItem;

            if (song == null) return;

            SongSearchResultsPopup.IsOpen = false;
            SearchTextBox.Text = "";

            OnPlaySong?.Invoke(song);
        }
        
        private void FocusSearch(
            KeyboardAccelerator sender,
            KeyboardAcceleratorInvokedEventArgs args
        )
        {
            SearchTextBox.Focus(FocusState.Keyboard);
            args.Handled = true;
        }

            
        public SearchBar()
        {
            InitializeComponent();
        }
    }
}

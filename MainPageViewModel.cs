using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.Web.Syndication;
using System.ComponentModel;

namespace Flashmobs
{
    class MainPageViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<FlashmobItem> Items
        {
            get;
            set;
        }


        public MainPageViewModel()
        {

            Items = new ObservableCollection<FlashmobItem>();
            Items.CollectionChanged += Items_CollectionChanged;
            Items.Add(new FlashmobItem()
                {
                    Time = "13:00",
                    Place = "Karlsruhe Marktplatz",
                    UniqueId = "asdKarlsruhe",
                    Name = "FlashmobKA"
                });

            //LoadFlashmobs();
        }

        void Items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged("Items");
        }

        //private async Task LoadFlashmobs()
        //{
        //    Items.Clear();
        //    List<FlashmobItem> FlashmobItems = await GetFeed("http://spiegel.de/schlagzeilen/tops/index.rss");

        //    foreach (var FlashmobItem in FlashmobItems)
        //    {
        //        Items.Add(FlashmobItem);
        //    }

        //}

        //private async Task<List<FlashmobItem>> GetFeed(string path)
        //{
        //    SyndicationClient client = new SyndicationClient();
        //    Uri uri = new Uri(path);
        //    SyndicationFeed feed = await client.RetrieveFeedAsync(uri);

        //    List<FlashmobItem> items = new List<FlashmobItem>();

        //    foreach (var feedItem in feed.Items)
        //    {
        //        FlashmobItem FlashmobItem = new FlashmobItem();
        //        FlashmobItem.Name = feedItem.Name;
        //        FlashmobItem.Time = feedItem.Title.Text;
        //        FlashmobItem.Place = feedItem.Title.Text;


        //        items.Add(FlashmobItem);
        //    }

        //    return items;
        //}

        protected virtual void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}

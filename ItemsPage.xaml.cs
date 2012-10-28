using Flashmobs.Data;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Die Elementvorlage für die Seite "Elemente" ist unter http://go.microsoft.com/fwlink/?LinkId=234233 dokumentiert.

namespace Flashmobs
{
    /// <summary>
    /// Eine Seite, auf der eine Auflistung von Elementvorschauen angezeigt wird. In der geteilten Anwendung wird diese Seite
    /// verwendet, um eine der verfügbaren Gruppen anzuzeigen und auszuwählen.
    /// </summary>
    public sealed partial class ItemsPage : Flashmobs.Common.LayoutAwarePage
    {
        public ItemsPage()
        {
            this.InitializeComponent();
            //this.DataContext = new MainPageViewModel();
        }

        /// <summary>
        /// Füllt die Seite mit Inhalt auf, der bei der Navigation übergeben wird. Gespeicherte Zustände werden ebenfalls
        /// bereitgestellt, wenn eine Seite aus einer vorherigen Sitzung neu erstellt wird.
        /// </summary>
        /// <param name="navigationParameter">Der Parameterwert, der an
        /// <see cref="Frame.Navigate(Type, Object)"/> übergeben wurde, als diese Seite ursprünglich angefordert wurde.
        /// </param>
        /// <param name="pageState">Ein Wörterbuch des Zustands, der von dieser Seite während einer früheren Sitzung
        /// beibehalten wurde. Beim ersten Aufrufen einer Seite ist dieser Wert NULL.</param>
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            // TODO: Ein geeignetes Datenmodell für die problematische Domäne erstellen, um die Beispieldaten auszutauschen
            var sampleDataGroups = SampleDataSource.GetGroups((String)navigationParameter);
            this.DefaultViewModel["Items"] = sampleDataGroups;
        }

        /// <summary>
        /// Wird aufgerufen, wenn auf ein Element geklickt wird.
        /// </summary>
        /// <param name="sender">GridView (oder ListView, wenn die Anwendung angedockt ist),
        /// die das angeklickte Element anzeigt.</param>
        /// <param name="e">Ereignisdaten, die das angeklickte Element beschreiben.</param>
        void ItemView_ItemClick(object sender, ItemClickEventArgs e)
        {
            // Zur entsprechenden Zielseite navigieren und die neue Seite konfigurieren,
            // indem die erforderlichen Informationen als Navigationsparameter übergeben werden
            var groupId = ((SampleDataItem)e.ClickedItem).Group.UniqueId;          

            this.Frame.Navigate(typeof(SplitPage), groupId);
        }
    }
}

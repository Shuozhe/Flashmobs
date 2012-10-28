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

// Die Elementvorlage "Geteilte Seite" ist unter http://go.microsoft.com/fwlink/?LinkId=234234 dokumentiert.

namespace Flashmobs
{
    /// <summary>
    /// Eine Seite, auf der ein Gruppentitel, eine Liste mit Elementen innerhalb der Gruppe sowie Details für das
    /// momentan ausgewählte Element angezeigt werden.
    /// </summary>
    public sealed partial class SplitPage : Flashmobs.Common.LayoutAwarePage
    {
        public SplitPage()
        {
            this.InitializeComponent();
        }

        #region Verwaltung des Seitenzustands

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
            var group = SampleDataSource.GetGroup((String)navigationParameter);
            this.DefaultViewModel["Group"] = group;
            this.DefaultViewModel["Items"] = group.Items;

            if (pageState == null)
            {
                this.itemListView.SelectedItem = null;
                // Wenn es sich hierbei um eine neue Seite handelt, das erste Element automatisch auswählen, außer wenn
                // logische Seitennavigation verwendet wird (weitere Informationen in der #Region zur logischen Seitennavigation unten).
                if (!this.UsingLogicalPageNavigation() && this.itemsViewSource.View != null)
                {
                    this.itemsViewSource.View.MoveCurrentToFirst();
                }
            }
            else
            {
                // Den zuvor gespeicherten Zustand wiederherstellen, der dieser Seite zugeordnet ist
                if (pageState.ContainsKey("SelectedItem") && this.itemsViewSource.View != null)
                {
                    var selectedItem = SampleDataSource.GetItem((String)pageState["SelectedItem"]);
                    this.itemsViewSource.View.MoveCurrentTo(selectedItem);
                }
            }
        }

        /// <summary>
        /// Behält den dieser Seite zugeordneten Zustand bei, wenn die Anwendung angehalten oder
        /// die Seite im Navigationscache verworfen wird. Die Werte müssen den Serialisierungsanforderungen
        /// von <see cref="SuspensionManager.SessionState"/> entsprechen.
        /// </summary>
        /// <param name="pageState">Ein leeres Wörterbuch, das mit dem serialisierbaren Zustand aufgefüllt wird.</param>
        protected override void SaveState(Dictionary<String, Object> pageState)
        {
            if (this.itemsViewSource.View != null)
            {
                var selectedItem = (SampleDataItem)this.itemsViewSource.View.CurrentItem;
                if (selectedItem != null) pageState["SelectedItem"] = selectedItem.UniqueId;
            }
        }

        #endregion

        #region Logische Seitennavigation

        // Die visuelle Zustandsverwaltung gibt die vier Ansichtszustände für Anwendungen normalerweise direkt wieder
        // (Vollbild im Querformat und Hochformat sowie angedockte und gefüllte Ansicht). Die geteilte Seite
        // wurde so entworfen, dass die angedockte Ansicht und das Querformat jeweils über zwei verschiedene Unterzustände verfügen:
        // entweder die Elementliste oder die Details werden angezeigt, jedoch nicht beides gleichzeitig.
        //
        // All dies wird mit einer einzigen physischen Seite implementiert, die zwei logische Seiten darstellen
        // kann. Mit dem nachfolgenden Code wird dieses Ziel erreicht, ohne dass der Benutzer aufmerksam gemacht wird auf den
        // Unterschied.

        /// <summary>
        /// Wird aufgerufen, um zu bestimmen, ob die Seite als eine logische Seite oder zwei agieren soll.
        /// </summary>
        /// <param name="viewState">Der Ansichtszustand, für den die Frage gestellt wird, oder NULL
        /// für den aktuellen Ansichtszustand. Dieser Parameter ist optional mit NULL als standardmäßigem
        /// Wert.</param>
        /// <returns>"True", wenn der fragliche Ansichtszustand Hochformat oder angedockt ist, "false"
        /// in anderen Fällen.</returns>
        private bool UsingLogicalPageNavigation(ApplicationViewState? viewState = null)
        {
            if (viewState == null) viewState = ApplicationView.Value;
            return viewState == ApplicationViewState.FullScreenPortrait ||
                viewState == ApplicationViewState.Snapped;
        }

        /// <summary>
        /// Wird aufgerufen, wenn ein Element innerhalb der Liste ausgewählt wird.
        /// </summary>
        /// <param name="sender">GridView (oder ListView, wenn die Anwendung angedockt ist),
        /// die das ausgewählte Element anzeigt.</param>
        /// <param name="e">Ereignisdaten, die beschreiben, wie die Auswahl geändert wurde.</param>
        void ItemListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Den Ansichtszustand ungültig machen, wenn die logische Seitennavigation wirksam ist, da eine Änderung an der
            // Auswahl zu einer entsprechenden Änderung an der aktuellen logischen Seite führen kann. Wenn
            // ein Element ausgewählt wird, führt dies dazu, dass anstelle der Elementliste
            // die Details des ausgewählten Elements angezeigt werden. Wenn die Auswahl aufgehoben wird, hat
            // dies den umgekehrten Effekt.
            if (this.UsingLogicalPageNavigation()) this.InvalidateVisualState();
        }

        /// <summary>
        /// Wird aufgerufen, wenn auf die Schaltfläche "Zurück" der Seite geklickt wird.
        /// </summary>
        /// <param name="sender">Die Instanz der Schaltfläche "Zurück".</param>
        /// <param name="e">Ereignisdaten, die beschreiben, wie auf die Schaltfläche "Zurück" geklickt wurde.</param>
        protected override void GoBack(object sender, RoutedEventArgs e)
        {
            if (this.UsingLogicalPageNavigation() && itemListView.SelectedItem != null)
            {
                // Wenn die logische Seitennavigation wirksam ist und ein ausgewähltes Element vorliegt, werden die
                // Details dieses Elements aktuell angezeigt. Beim Aufheben der Auswahl wird die
                // Elementliste wieder aufgehoben. Aus Sicht des Benutzers ist dies eine logische
                // Rückwärtsnavigation.
                this.itemListView.SelectedItem = null;
            }
            else
            {
                // Wenn die logische Seitennavigation nicht wirksam ist oder kein ausgewähltes Element
                // vorliegt, das Standardverhalten der Schaltfläche "Zurück" verwenden.
                base.GoBack(sender, e);
            }
        }

        /// <summary>
        /// Wird aufgerufen, um den Namen des visuellen Zustands zu bestimmen, der dem Ansichtszustand einer Anwendung
        /// entspricht.
        /// </summary>
        /// <param name="viewState">Der Ansichtszustand, für den die Frage gestellt wird.</param>L
        /// <returns>Der Name des gewünschten visuellen Zustands. Dieser ist identisch mit dem Namen des
        /// Ansichtszustands, außer wenn ein ausgewähltes Element im Hochformat und in der angedockten Ansicht vorliegt, wobei
        /// diese zusätzliche logische Seite durch Hinzufügen des Suffix _Detail dargestellt wird.</returns>
        protected override string DetermineVisualState(ApplicationViewState viewState)
        {
            // Den Aktivierungszustand der Schaltfläche "Zurück" aktualisieren, wenn der Ansichtszustand geändert wird
            var logicalPageBack = this.UsingLogicalPageNavigation(viewState) && this.itemListView.SelectedItem != null;
            var physicalPageBack = this.Frame != null && this.Frame.CanGoBack;
            this.DefaultViewModel["CanGoBack"] = logicalPageBack || physicalPageBack;

            // Visuelle Zustände für Querformatlayouts nicht auf dem Ansichtszustand, sondern der Breite
            // des Fensters basierend festlegen. Für diese Seite gibt es ein Layout, das für
            // 1366 virtuelle Pixel oder breiter geeignet ist, und ein anderes für schmalere Anzeigen oder wenn eine angedockte
            // Anwendung den verfügbaren horizontalen Raum auf weniger als 1366 Pixel reduziert.
            if (viewState == ApplicationViewState.Filled ||
                viewState == ApplicationViewState.FullScreenLandscape)
            {
                var windowWidth = Window.Current.Bounds.Width;
                if (windowWidth >= 1366) return "FullScreenLandscapeOrWide";
                return "FilledOrNarrow";
            }

            // Im Hochformat oder bei angedockter Anwendung mit dem visuellen Standardzustandsnamen starten, dann ein
            // Suffix hinzufügen, wenn Details anstatt der Liste angezeigt werden
            var defaultStateName = base.DetermineVisualState(viewState);
            return logicalPageBack ? defaultStateName + "_Detail" : defaultStateName;
        }

        #endregion

        private void joinMob(object sender, RoutedEventArgs e)
        {
            joinedText.Text = "Ich bin dabei!";
        }

        private void cancelMob(object sender, RoutedEventArgs e)
        {
            joinedText.Text = "";
        }

        private void discardFlashmob(object sender, RoutedEventArgs e)
        {
            String test = itemTitle.Text;
            SampleDataSource.DeleteItem(((SampleDataItem)itemDetail.DataContext).UniqueId);
            itemDetail.UpdateLayout();
        }
    }
}

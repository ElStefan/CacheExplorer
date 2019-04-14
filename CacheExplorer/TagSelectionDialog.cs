using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CacheExplorer.Extensions;
using CacheExplorer.Helper;
using CacheExplorer.Model;

namespace CacheExplorer
{
    public partial class TagSelectionDialog : Form
    {
        public TagSelectionDialog(Result originalMatch, IEnumerable<Result> iTunesSuggestions)
        {
            InitializeComponent();

            var artist = originalMatch.artistName;
            var title =  originalMatch.trackName;
            var album =  originalMatch.collectionName;

            this.textBoxTitle.Text = title;
            this.textBoxArtist.Text = artist;
            this.textBoxAlbum.Text = album;
            if (originalMatch.GpmdpDataAvailable)
            {
                labelSource.Text = "Google Play Music";
                if(!string.IsNullOrEmpty(originalMatch.lyrics))
                {
                    labelSource.Text += " (with lyrics)";
                }
            }
            this.fastObjectListViewMatches.SetObjects(iTunesSuggestions?.ToList() ?? new List<Result>());
        }

        public Result SelectedTag { get; internal set; }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            this.SelectTag();
        }

        private void SelectTag()
        {
            this.SelectedTag = this.fastObjectListViewMatches.SelectedObject as Result;
            if(this.SelectedTag == null)
            {
                MessageBox.Show("No tag selected");
                return;
            }
            DialogResult = DialogResult.OK;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void buttonSearch_Click(object sender, EventArgs e)
        {
            var iTunesSuggestions = iTunesHelper.GetResults(this.textBoxArtist.Text, this.textBoxTitle.Text, this.textBoxAlbum.Text);
            iTunesSuggestions = iTunesSuggestions.OrderBy(o => o.trackName.Similarity(this.textBoxTitle.Text)).ThenBy(o => o.collectionName.Similarity(this.textBoxAlbum.Text));
            this.fastObjectListViewMatches.SetObjects(iTunesSuggestions.ToList());
        }
    }
}

using CacheExplorer.Extensions;
using CacheExplorer.Helper;
using CacheExplorer.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace CacheExplorer
{
  public partial class TagSelectionDialog : Form
  {
    public TagSelectionDialog(Result originalMatch, IEnumerable<Result> iTunesSuggestions)
    {
      InitializeComponent();

      var artist = originalMatch.artistName;
      var title = originalMatch.trackName;
      var album = originalMatch.collectionName;

      textBoxTitle.Text = title;
      textBoxArtist.Text = artist;
      textBoxAlbum.Text = album;
      if (originalMatch.AllInfosGiven)
      {
        labelSource.Text = "Existing file tags";
        if (!string.IsNullOrEmpty(originalMatch.lyrics))
        {
          labelSource.Text += " (with lyrics)";
        }
      }

      fastObjectListViewMatches.SetObjects(iTunesSuggestions?.ToList() ?? new List<Result>());
      fastObjectListViewMatches.SelectedObject = iTunesSuggestions?.FirstOrDefault();
    }

    public Result SelectedTag { get; internal set; }

    private void buttonOk_Click(object sender, EventArgs e)
    {
      SelectTag();
    }

    private void SelectTag()
    {
      SelectedTag = fastObjectListViewMatches.SelectedObject as Result;
      if (SelectedTag == null)
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
      var iTunesSuggestions = iTunesHelper.GetResults(textBoxArtist.Text, textBoxTitle.Text, textBoxAlbum.Text);
      iTunesSuggestions = iTunesSuggestions.OrderBy(o => o.trackName.Similarity(textBoxTitle.Text)).ThenBy(o => o.collectionName.Similarity(textBoxAlbum.Text));
      fastObjectListViewMatches.SetObjects(iTunesSuggestions.ToList());
    }
  }
}

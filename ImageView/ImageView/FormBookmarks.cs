﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeneralToolkitLib.Converters;
using GeneralToolkitLib.Log;
using ImageView.DataBinding;
using ImageView.DataContracts;
using ImageView.Events;
using ImageView.InputForms;
using ImageView.Managers;
using ImageView.Properties;
using ImageView.Services;
using ImageView.UserControls;
using ImageView.Utility;

namespace ImageView
{
    public partial class FormBookmarks : Form
    {
        private const int CustomContentHeight = 4;
        private readonly Color _gridViewGradientBackgroundColorStart = ColorTranslator.FromHtml("#b2e1ff");
        private readonly Color _gridViewGradientBackgroundColorStop = ColorTranslator.FromHtml("#66b6fc");
        private readonly Color _gridViewSelectionBorderColor = ColorTranslator.FromHtml("#7da2ce");
        private readonly StringBuilder _logStringBuilder;
        private readonly List<string> _volumeInfoArray;
        private TreeViewDataContext _treeViewDataContext;
        private int _brokenLinks;
        private string _defaultDrive = "c:\\";

        private Rectangle _dragBoxFromMouseDown;
        private int _fixedLinks;
        private object _valueFromMouseDown;
        private readonly BookmarkService _bookmarkService;
        private readonly BookmarkManager _bookmarkManager;
        private readonly ApplicationSettingsService _applicationSettingsService;

        public FormBookmarks(BookmarkService bookmarkService, BookmarkManager bookmarkManager, ApplicationSettingsService applicationSettingsService)
        {
            _bookmarkService = bookmarkService;
            _bookmarkManager = bookmarkManager;
            _applicationSettingsService = applicationSettingsService;
            InitializeComponent();
            _logStringBuilder = new StringBuilder();
            _volumeInfoArray = new List<string>();

            try
            {
                var driveInfoArray = DriveInfo.GetDrives();
                foreach (DriveInfo driveInfo in driveInfoArray.Where(driveInfo => driveInfo.IsReady))
                {
                    _volumeInfoArray.Add(driveInfo.Name);
                }
            }
            catch (Exception ex)
            {
                LogWriter.LogError("FormBookmarks() Constructor", ex);
            }
        }

        private void FormBookmarks_Load(object sender, EventArgs e)
        {
            if (DesignMode)
                return;
            
            bookmarksDataGridView.RowPrePaint += bookmarksDataGridView_RowPrePaint;
            bookmarksDataGridView.RowPostPaint += bookmarksDataGridView_RowPostPaint;
            _bookmarkManager.OnBookmarksUpdate += Instance_OnBookmarksUpdate;
            bookmarksTree.AfterSelect += BookmarksTree_AfterSelect;
            InitBookmarksDataGridViev();
            _treeViewDataContext = new TreeViewDataContext(bookmarksTree, _bookmarkManager.RootFolder);

            if (_applicationSettingsService.Settings.PasswordProtectBookmarks)
                using (var formgetPassword = new FormGetPassword
                {
                    PasswordDerivedString = _applicationSettingsService.Settings.PasswordDerivedString
                })
                {
                    if (formgetPassword.ShowDialog() == DialogResult.OK)
                    {
                        if (!formgetPassword.PasswordVerified)
                        {
                            MessageBox.Show(this, Resources.Invalid_password_);
                            Close();
                            return;
                        }

                        if (_bookmarkService.OpenBookmarks(formgetPassword.PasswordString))
                            InitBookmarksDataSource();
                        else
                        {
                            MessageBox.Show(Resources.failed_to_decode_file_);
                            Close();
                        }
                    }
                    else
                        Close();
                }
            else
            {
                if (!_bookmarkManager.LoadedFromFile)
                {
                    _bookmarkService.OpenBookmarks();
                }

                InitBookmarksDataSource();
            }
        }

        private void BookmarksTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            ReLoadBookmarks();
        }

        private void Instance_OnBookmarksUpdate(object sender, BookmarkUpdatedEventArgs e)
        {
            if (bookmarksDataGridView.SelectedRows.Count > 0)
            {
                DataGridViewSelectedRowCollection selectedItems = bookmarksDataGridView.SelectedRows;
                var selectionList = new List<int>();
                for (int i = 0; i < selectedItems.Count; i++)
                {
                    selectionList.Add(selectedItems[i].Index);
                }

                ReLoadBookmarks(selectionList);
                return;
            }

            ReLoadBookmarks();
        }

        private void ReLoadBookmarks(IEnumerable<int> selectedIndexList)
        {
            if (!ReLoadBookmarks()) return;
            foreach (int index in selectedIndexList)
            {
                if (index >= bookmarksDataGridView.Rows.Count)
                    break;

                bookmarksDataGridView.Rows[index].Selected = true;
            }
        }

        private bool ReLoadBookmarks()
        {
            TreeNode selectedNode = bookmarksTree.SelectedNode;

            if (!(selectedNode.Tag is BookmarkFolder selectedBookmarkfolder)) return false;
            bookmarkBindingSource.DataSource = selectedBookmarkfolder.Bookmarks.OrderBy(x => x.SortOrder).ToList();
        
            //bookmarksDataGridView.DataSource = selectedBookmarkfolder.Bookmarks.OrderBy(x => x.SortOrder).ToList();
            bookmarksDataGridView.Update();
            bookmarksDataGridView.Refresh();

            return true;
        }

        private void AlterTreeViewState(TreeViewFolderStateChange stateChange, BookmarkFolder folder)
        {
            if (stateChange == TreeViewFolderStateChange.FolderAdded)
            {
                _treeViewDataContext.ExpandNode(folder);
            }
            else if (stateChange == TreeViewFolderStateChange.FolderRenamed)
            {
                InitBookmarksDataSource();
                _treeViewDataContext.ExpandNode(folder);
            }
            else
            {
                _treeViewDataContext.ExpandNode(bookmarksTree.TopNode.Tag as BookmarkFolder);
            }
        }

        private void InitBookmarksDataSource()
        {
            _treeViewDataContext = new TreeViewDataContext(bookmarksTree, _bookmarkManager.RootFolder);
            _treeViewDataContext.BindData();
        }

        private void InitBookmarksDataGridViev()
        {
            // Set a cell padding to provide space for the top of the focus 
            // rectangle and for the content that spans multiple columns. 
            var newPadding = new Padding(0, 1, 0, CustomContentHeight);
            bookmarksDataGridView.RowTemplate.DefaultCellStyle.Padding = newPadding;

            // Set the selection background color to transparent so 
            // the cell won't paint over the custom selection background.
            bookmarksDataGridView.RowTemplate.DefaultCellStyle.SelectionBackColor = Color.Transparent;

            // Set the row height to accommodate the content that 
            // spans multiple columns.
            bookmarksDataGridView.RowTemplate.Height += CustomContentHeight;

            // Initialize other DataGridView properties.
            bookmarksDataGridView.CellBorderStyle = DataGridViewCellBorderStyle.None;
            bookmarksDataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }

        // Paints the content that spans multiple columns and the focus rectangle.
        private void bookmarksDataGridView_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            SolidBrush forebrush = null;
            try
            {
                // Determine the foreground color.
                forebrush = (e.State & DataGridViewElementStates.Selected) == DataGridViewElementStates.Selected
                    ? new SolidBrush(e.InheritedRowStyle.SelectionForeColor)
                    : new SolidBrush(e.InheritedRowStyle.ForeColor);
            }
            finally
            {
                forebrush?.Dispose();
            }
        }

        private void bookmarksDataGridView_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            try
            {
                // Do not automatically paint the focus rectangle.
                e.PaintParts &= ~DataGridViewPaintParts.Focus;

                // Determine whether the cell should be painted
                // with the custom selection background.
                if ((e.State & DataGridViewElementStates.Selected) == DataGridViewElementStates.Selected)
                {
                    // Calculate the bounds of the row.
                    var rowBounds = new Rectangle(0, e.RowBounds.Top,
                        bookmarksDataGridView.Columns.GetColumnsWidth(DataGridViewElementStates.Visible) -
                        bookmarksDataGridView.HorizontalScrollingOffset + 1,
                        e.RowBounds.Height);

                    // Paint the custom selection background.
                    using (
                        Brush backbrush = new LinearGradientBrush(rowBounds, _gridViewGradientBackgroundColorStart,
                            _gridViewGradientBackgroundColorStop, LinearGradientMode.Vertical))
                    {
                        e.Graphics.FillRectangle(backbrush, rowBounds);
                        var p = new Pen(backbrush, 1) {Color = _gridViewSelectionBorderColor};
                        e.Graphics.DrawRectangle(p, rowBounds);
                    }
                }
            }
            catch (Exception ex)
            {
                LogWriter.LogError("Exception in bookmarksDataGridView_RowPrePaint()", ex);
            }
        }

        private void bookmarksDataGridView_KeyDown(object sender, KeyEventArgs e)
        {
            if (bookmarksDataGridView.SelectedRows.Count != 1) return;
            DataGridViewRow selectedRow = bookmarksDataGridView.CurrentRow;
            BookmarkManager bookmarkManager = _bookmarkManager;

            if (e.KeyData == Keys.Enter)
            {
                e.Handled = true;
                LoadImageFromSelectedRow();
            }
            if (e.KeyData != Keys.Delete || selectedRow == null || MessageBox.Show(this, Resources.Are_you_sure_you_want_to_delete_this_bookmark_, Resources.Delete, MessageBoxButtons.YesNo) != DialogResult.Yes) return;

            if (!(selectedRow.DataBoundItem is Bookmark bookmark)) return;
            bookmarkManager.DeleteBookmark(bookmark);
            bookmarksDataGridView.DataSource = bookmarkManager.RootFolder.Bookmarks.OrderBy(x => x.SortOrder).ToList();
        }

        private void LoadImageFromSelectedRow()
        {
            if (bookmarksDataGridView.SelectedRows.Count != 1) return;

            DataGridViewRow selectedRow = bookmarksDataGridView.CurrentRow;

            if (!(selectedRow?.DataBoundItem is Bookmark bookmark)) return;
            try
            {
                Process.Start(bookmark.CompletePath);
            }
            catch (Exception ex)
            {
                LogWriter.LogError("Error in LoadImageFromSelectedRow()", ex);
            }
        }

        private void LoadPreviewImageFromSelectedRow()
        {
            if (bookmarksDataGridView.SelectedRows.Count != 1) return;

            DataGridViewRow selectedRow = bookmarksDataGridView.CurrentRow;

            if (selectedRow?.DataBoundItem is Bookmark bookmark)
                pictureBoxPreview.ImageLocation = bookmark.CompletePath;
        }

        private void bookmarksDataGridView_SelectionChanged(object sender, EventArgs e)
        {
            LoadPreviewImageFromSelectedRow();
        }

        private void bookmarksDataGridView_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                LoadImageFromSelectedRow();
        }

        private void bookmarksDataGridView_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                contextMenuStripBookmarks.Show(bookmarksDataGridView, new Point(e.X, e.Y));
        }

        private void addFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (bookmarksTree.SelectedNode == null)
                return;
           
            var inputFormData = new FormInputRow.InputFormData
            {
                GroupBoxText = "Bookmark folder name",
                LabelText = "Name:",
                WindowText = "Add new bookmark folder"
            };
            var formInputRow = new FormInputRow(inputFormData);

            if (formInputRow.ShowDialog(this) == DialogResult.OK)
            {
                string folderName = formInputRow.UserInputText;
                TreeNode selectedNode = bookmarksTree.SelectedNode;

                if (selectedNode.Tag is BookmarkFolder selectedBookmarkfolder)
                {
                    BookmarkFolder newFolder = _bookmarkManager.AddBookmarkFolder(selectedBookmarkfolder.Id, folderName);
                    AlterTreeViewState(TreeViewFolderStateChange.FolderAdded, newFolder);
                }
            }
        }

        private void deleteFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!(bookmarksTree.SelectedNode?.Tag is BookmarkFolder treeNode))
                return;

            if (
                MessageBox.Show(this, Resources.Are_you_sure_you_want_to_delete_this_folder_, Resources.Remove_folder_,
                    MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                _bookmarkManager.DeleteBookmarkFolder(treeNode);
                AlterTreeViewState(TreeViewFolderStateChange.FolderRemoved, treeNode);
            }
        }

        private void renameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataGridViewRow selectedRow = bookmarksDataGridView.CurrentRow;
            if (!(selectedRow?.DataBoundItem is Bookmark bookmark)) return;

            var editBookmark = new FormEditBookmark();
            editBookmark.InitForRename(bookmark.BoookmarkName, bookmark.FileName);
            if (editBookmark.ShowDialog(this) == DialogResult.OK)
            {
                string name = editBookmark.GetNewName();
                bookmark.BoookmarkName = name;
                ReLoadBookmarks();
            }
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataGridViewRow selectedRow = bookmarksDataGridView.CurrentRow;
            if (!(selectedRow?.DataBoundItem is Bookmark bookmark)) return;
            
            _bookmarkManager.DeleteBookmark(bookmark);
            ReLoadBookmarks();
        }

        private bool VolumeExists(string volumeLabel)
        {
            if (string.IsNullOrEmpty(volumeLabel))
                return false;

            return _volumeInfoArray.Any(x => String.Equals(x, volumeLabel, StringComparison.CurrentCultureIgnoreCase));
        }

        private void tryToFixBrokenLinksToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_bookmarkManager == null)
                return;

            showLogToolStripMenuItem.Enabled = true;
            _logStringBuilder.Clear();
            _brokenLinks = 0;
            _fixedLinks = 0;

            BookmarkFolder bookmarkFolder = _bookmarkManager.RootFolder;
            UpdateBrokenLinksOnBookmarks(bookmarkFolder.Bookmarks);
            UpdateBrokenLinksOnThreeNodes(bookmarkFolder.BookmarkFolders);

            _logStringBuilder.AppendLine($"Found {_brokenLinks} broken linkes and fixed {_fixedLinks} links.");

            if (_fixedLinks > 0)
                _bookmarkService.SaveBookmarks();
        }

        private void UpdateBrokenLinksOnThreeNodes(List<BookmarkFolder> bookmarkTreeNodes)
        {
            if (bookmarkTreeNodes == null)
                return;

            foreach (BookmarkFolder bookmarkTreeNode in bookmarkTreeNodes)
            {
                UpdateBrokenLinksOnBookmarks(bookmarkTreeNode.Bookmarks);
                UpdateBrokenLinksOnThreeNodes(bookmarkTreeNode.BookmarkFolders);
            }
        }

        private void UpdateBrokenLinksOnBookmarks(List<Bookmark> bookmarks)
        {
            if (bookmarks == null)
                return;

            foreach (Bookmark bookmark in bookmarks)
            {
                string volumeLabel = GeneralConverters.GetVolumeLabelFromPath(bookmark.Directory);
                string originalPath = bookmark.CompletePath;
                if (!VolumeExists(volumeLabel)) continue;
                if (!File.Exists(bookmark.CompletePath))
                {
                    _brokenLinks++;
                    if (!FindFilePath(bookmark, _defaultDrive)) continue;
                    _logStringBuilder.AppendLine($"Fixed broken link: {originalPath} replaced to {bookmark.CompletePath}");
                    _fixedLinks++;
                }
            }
        }

        private bool FindFilePath(Bookmark bookmark, string rootDirectory)
        {
            var directoryInfo = new DirectoryInfo(rootDirectory);
            FileInfo[] fileInfoArray = null;
            try
            {
                fileInfoArray = directoryInfo.GetFiles(bookmark.FileName, SearchOption.AllDirectories);
            }
            catch (Exception exception)
            {
                LogWriter.LogError("Exception in FindFilePath() rootDirectory=" + rootDirectory, exception);
            }

            FileInfo fileInfo =
                fileInfoArray?.FirstOrDefault(fi => fi.Name == bookmark.FileName && fi.Length == bookmark.Size);
            if (fileInfo == null) return false;
            bookmark.Directory = fileInfo.DirectoryName;
            bookmark.CompletePath = fileInfo.DirectoryName + "\\" + bookmark.FileName;
            bookmark.LastAccessTime = fileInfo.LastAccessTime;

            return true;
        }

        private void showLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var formLogWindow = new FormLogWindow();
            formLogWindow.SetLogText(_logStringBuilder.ToString());
            formLogWindow.ShowDialog(this);
        }

        private void setDefaultDriveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var formSetDefaultDrive = new FormSetDefaultDrive();
            if (formSetDefaultDrive.ShowDialog(this) == DialogResult.OK)
                _defaultDrive = formSetDefaultDrive.SelectedDrive;
        }

        private void renameFolderMenuItem_Click(object sender, EventArgs e)
        {
            if (bookmarksTree.SelectedNode?.Tag is BookmarkFolder bookmarkFolder)
            {
                var ucRenameFolder = new RenameBookmarkFolder();
                ucRenameFolder.InitControl(bookmarkFolder.Name, bookmarkFolder.Bookmarks.Count);
                Form renameForm = FormFactory.CreateModalForm(ucRenameFolder);

                if (renameForm.ShowDialog(this) == DialogResult.OK)
                {
                    string newName = ucRenameFolder.GetNewFolderName();
                    bookmarkFolder.Name = newName;
                    AlterTreeViewState(TreeViewFolderStateChange.FolderRenamed, bookmarkFolder);
                }
            }
        }

        private void bookmarksTree_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetData(typeof(Bookmark)) is Bookmark bookmark)
            {
                // The mouse locations are relative to the screen, so they must be 
                // converted to client coordinates.
                Point clientPoint = bookmarksTree.PointToClient(new Point(e.X, e.Y));

                // If the drag operation was a move then add the row to the other control.
                if (e.Effect == DragDropEffects.Move)
                {
                    TreeViewHitTestInfo hittest = bookmarksTree.HitTest(clientPoint.X, clientPoint.Y);
                    if (hittest.Node == null) return;
                    TreeNode dropNode = hittest.Node;
                    if (!(dropNode.Tag is BookmarkFolder dropFolder)) return;

                    _bookmarkManager.MoveBookmark(bookmark, dropFolder.Id);
                    ReLoadBookmarks();
                }
            }
        }

        private void bookmarksTree_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = e.Data.GetDataPresent(typeof(Bookmark)) ? DragDropEffects.Move : DragDropEffects.None;
        }

        private void bookmarksTree_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(Bookmark)))
                e.Effect = e.AllowedEffect;
        }

        private void bookmarksDataGridView_MouseDown(object sender, MouseEventArgs e)
        {
            // Get the index of the item the mouse is below.
            DataGridView.HitTestInfo hittestInfo = bookmarksDataGridView.HitTest(e.X, e.Y);

            if (hittestInfo.RowIndex != -1 && hittestInfo.ColumnIndex != -1)
            {
                _valueFromMouseDown = bookmarksDataGridView.Rows[hittestInfo.RowIndex].DataBoundItem;
                if (_valueFromMouseDown != null)
                {
                    // Remember the point where the mouse down occurred. 
                    // The DragSize indicates the size that the mouse can move 
                    // before a drag event should be started.                
                    Size dragSize = SystemInformation.DragSize;

                    // Create a rectangle using the DragSize, with the mouse position being
                    // at the center of the rectangle.
                    _dragBoxFromMouseDown = new Rectangle(new Point(e.X - dragSize.Width/2, e.Y - dragSize.Height/2), dragSize);
                }
            }
            else
            // Reset the rectangle if the mouse is not over an item in the ListBox.
                _dragBoxFromMouseDown = Rectangle.Empty;
        }

        private void bookmarksDataGridView_MouseMove(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
            {
                // If the mouse moves outside the rectangle, start the drag.
                if (_dragBoxFromMouseDown != Rectangle.Empty && !_dragBoxFromMouseDown.Contains(e.X, e.Y))
                {
                    // Proceed with the drag and drop, passing in the list item.                    
                    bookmarksDataGridView.DoDragDrop(_valueFromMouseDown, DragDropEffects.Move);
                }
            }
        }

        private enum TreeViewFolderStateChange
        {
            FolderRemoved,
            FolderAdded,
            FolderRenamed
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = ".dat|BookmarkFiles";
            openFileDialog1.FileName = "bookmarks.dat";
            if (openFileDialog1.ShowDialog(this) == DialogResult.OK)
            {
                string filename = openFileDialog1.FileName;

            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool result =_bookmarkService.SaveBookmarks();
            if (result)
            {
                MessageBox.Show("Bookmarks saved", "Bookmarks", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Unable to save bookmarks", "Bookmarks", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Filter= ".dat|BookmarkFiles";
            saveFileDialog1.FileName = "bookmarks.dat";

            var userControl = new SelectPassword();
            Form paswordForm = FormFactory.CreateModalForm(userControl);
            
            paswordForm.Controls.Add(userControl);
            paswordForm.StartPosition = FormStartPosition.CenterParent;
            paswordForm.ShowInTaskbar = false;
            paswordForm.FormBorderStyle = FormBorderStyle.FixedSingle;


            if (paswordForm.ShowDialog(this) == DialogResult.OK)
            {
                string password = userControl.SelectedPassword;

                if (saveFileDialog1.ShowDialog(this) == DialogResult.OK)
                {
                    string filename = saveFileDialog1.FileName;
                    _bookmarkManager.SaveToFile(filename, password);
                    MessageBox.Show("Save was sussessful", "Bookmarks", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            
        }

        private void removeDuplicatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _bookmarkManager.RemoveDuplicates();
            ReLoadBookmarks();
            MessageBox.Show(this, "Duplicate bookmarks pointing to the same file where removed", "Bookmarks", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void bookmarkBindingSource_CurrentChanged(object sender, EventArgs e)
        {

        }
    }
}
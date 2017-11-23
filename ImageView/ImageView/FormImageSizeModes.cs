﻿using System;
using System.Windows.Forms;
using GeneralToolkitLib.Log;
using ImageView.Services;

namespace ImageView
{
    public partial class FormImageSizeModes : Form
    {
        private readonly ApplicationSettingsService _applicationSettingsService;
        public FormImageSizeModes(ApplicationSettingsService applicationSettingsService)
        {
            _applicationSettingsService = applicationSettingsService;
            InitializeComponent();
        }

        public PictureBoxSizeMode ImageSizeMode { get; private set; }

        private void FormImageSizeModes_Load(object sender, EventArgs e)
        {
            comboBoxImageSizeModes.DataSource = Enum.GetValues(typeof(PictureBoxSizeMode));
            int sizeMode =_applicationSettingsService.Settings.PrimaryImageSizeMode;

            try
            {
                comboBoxImageSizeModes.SelectedIndex = sizeMode;
            }
            catch (Exception ex)
            {
                LogWriter.LogError("Invalid image Size mode in app settings. Actual value:" + sizeMode, ex);
               _applicationSettingsService.Settings.PrimaryImageSizeMode = 0;
            }
        }

        private void comboBoxImageSizeModes_SelectedIndexChanged(object sender, EventArgs e)
        {
            ImageSizeMode =
                (PictureBoxSizeMode)
                    Enum.Parse(typeof(PictureBoxSizeMode), comboBoxImageSizeModes.SelectedIndex.ToString());
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Abort;
            Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
﻿namespace NAudioDemo.RecordingDemo
{
    partial class RecordingPanel
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RecordingPanel));
            this.buttonStartRecording = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonStopRecording = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.label2 = new System.Windows.Forms.Label();
            this.listBoxRecordings = new System.Windows.Forms.ListBox();
            this.buttonPlay = new System.Windows.Forms.Button();
            this.buttonDelete = new System.Windows.Forms.Button();
            this.buttonOpenFolder = new System.Windows.Forms.Button();
            this.groupBoxRecordingApi = new System.Windows.Forms.GroupBox();
            this.checkBoxEventCallback = new System.Windows.Forms.CheckBox();
            this.comboBoxChannels = new System.Windows.Forms.ComboBox();
            this.comboBoxSampleRate = new System.Windows.Forms.ComboBox();
            this.comboWaveInDevice = new System.Windows.Forms.ComboBox();
            this.comboWasapiDevices = new System.Windows.Forms.ComboBox();
            this.radioButtonWasapiLoopback = new System.Windows.Forms.RadioButton();
            this.radioButtonWasapi = new System.Windows.Forms.RadioButton();
            this.radioButtonWaveIn = new System.Windows.Forms.RadioButton();
            this.comboWasapiLoopbackDevices = new System.Windows.Forms.ComboBox();
            this.groupBoxRecordingApi.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonStartRecording
            // 
            this.buttonStartRecording.Location = new System.Drawing.Point(314, 96);
            this.buttonStartRecording.Name = "buttonStartRecording";
            this.buttonStartRecording.Size = new System.Drawing.Size(105, 23);
            this.buttonStartRecording.TabIndex = 0;
            this.buttonStartRecording.Text = "Start Recording";
            this.buttonStartRecording.UseVisualStyleBackColor = true;
            this.buttonStartRecording.Click += new System.EventHandler(this.OnButtonStartRecordingClick);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.Location = new System.Drawing.Point(12, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(590, 44);
            this.label1.TabIndex = 1;
            this.label1.Text = resources.GetString("label1.Text");
            // 
            // buttonStopRecording
            // 
            this.buttonStopRecording.Location = new System.Drawing.Point(436, 96);
            this.buttonStopRecording.Name = "buttonStopRecording";
            this.buttonStopRecording.Size = new System.Drawing.Size(105, 23);
            this.buttonStopRecording.TabIndex = 0;
            this.buttonStopRecording.Text = "Stop Recording";
            this.buttonStopRecording.UseVisualStyleBackColor = true;
            this.buttonStopRecording.Click += new System.EventHandler(this.OnButtonStopRecordingClick);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(314, 155);
            this.progressBar1.Maximum = 30;
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(257, 23);
            this.progressBar1.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(311, 136);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(103, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Recording Progress:";
            // 
            // listBoxRecordings
            // 
            this.listBoxRecordings.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBoxRecordings.FormattingEnabled = true;
            this.listBoxRecordings.Location = new System.Drawing.Point(15, 245);
            this.listBoxRecordings.Name = "listBoxRecordings";
            this.listBoxRecordings.Size = new System.Drawing.Size(506, 108);
            this.listBoxRecordings.TabIndex = 8;
            // 
            // buttonPlay
            // 
            this.buttonPlay.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonPlay.Location = new System.Drawing.Point(527, 272);
            this.buttonPlay.Name = "buttonPlay";
            this.buttonPlay.Size = new System.Drawing.Size(75, 23);
            this.buttonPlay.TabIndex = 9;
            this.buttonPlay.Text = "Play";
            this.buttonPlay.UseVisualStyleBackColor = true;
            this.buttonPlay.Click += new System.EventHandler(this.OnButtonPlayClick);
            // 
            // buttonDelete
            // 
            this.buttonDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonDelete.Location = new System.Drawing.Point(527, 301);
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.Size = new System.Drawing.Size(75, 23);
            this.buttonDelete.TabIndex = 10;
            this.buttonDelete.Text = "Delete";
            this.buttonDelete.UseVisualStyleBackColor = true;
            this.buttonDelete.Click += new System.EventHandler(this.OnButtonDeleteClick);
            // 
            // buttonOpenFolder
            // 
            this.buttonOpenFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOpenFolder.Location = new System.Drawing.Point(527, 330);
            this.buttonOpenFolder.Name = "buttonOpenFolder";
            this.buttonOpenFolder.Size = new System.Drawing.Size(75, 23);
            this.buttonOpenFolder.TabIndex = 10;
            this.buttonOpenFolder.Text = "Open Folder";
            this.buttonOpenFolder.UseVisualStyleBackColor = true;
            this.buttonOpenFolder.Click += new System.EventHandler(this.OnOpenFolderClick);
            // 
            // groupBoxRecordingApi
            // 
            this.groupBoxRecordingApi.Controls.Add(this.comboWasapiLoopbackDevices);
            this.groupBoxRecordingApi.Controls.Add(this.checkBoxEventCallback);
            this.groupBoxRecordingApi.Controls.Add(this.comboBoxChannels);
            this.groupBoxRecordingApi.Controls.Add(this.comboBoxSampleRate);
            this.groupBoxRecordingApi.Controls.Add(this.comboWaveInDevice);
            this.groupBoxRecordingApi.Controls.Add(this.comboWasapiDevices);
            this.groupBoxRecordingApi.Controls.Add(this.radioButtonWasapiLoopback);
            this.groupBoxRecordingApi.Controls.Add(this.radioButtonWasapi);
            this.groupBoxRecordingApi.Controls.Add(this.radioButtonWaveIn);
            this.groupBoxRecordingApi.Location = new System.Drawing.Point(15, 66);
            this.groupBoxRecordingApi.Name = "groupBoxRecordingApi";
            this.groupBoxRecordingApi.Size = new System.Drawing.Size(269, 173);
            this.groupBoxRecordingApi.TabIndex = 11;
            this.groupBoxRecordingApi.TabStop = false;
            this.groupBoxRecordingApi.Text = "Recording API";
            // 
            // checkBoxEventCallback
            // 
            this.checkBoxEventCallback.AutoSize = true;
            this.checkBoxEventCallback.Location = new System.Drawing.Point(141, 41);
            this.checkBoxEventCallback.Name = "checkBoxEventCallback";
            this.checkBoxEventCallback.Size = new System.Drawing.Size(103, 17);
            this.checkBoxEventCallback.TabIndex = 13;
            this.checkBoxEventCallback.Text = "Event Callbacks";
            this.checkBoxEventCallback.UseVisualStyleBackColor = true;
            // 
            // comboBoxChannels
            // 
            this.comboBoxChannels.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxChannels.FormattingEnabled = true;
            this.comboBoxChannels.Location = new System.Drawing.Point(141, 67);
            this.comboBoxChannels.Name = "comboBoxChannels";
            this.comboBoxChannels.Size = new System.Drawing.Size(121, 21);
            this.comboBoxChannels.TabIndex = 12;
            // 
            // comboBoxSampleRate
            // 
            this.comboBoxSampleRate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxSampleRate.FormattingEnabled = true;
            this.comboBoxSampleRate.Location = new System.Drawing.Point(15, 67);
            this.comboBoxSampleRate.Name = "comboBoxSampleRate";
            this.comboBoxSampleRate.Size = new System.Drawing.Size(121, 21);
            this.comboBoxSampleRate.TabIndex = 12;
            // 
            // comboWaveInDevice
            // 
            this.comboWaveInDevice.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboWaveInDevice.FormattingEnabled = true;
            this.comboWaveInDevice.Location = new System.Drawing.Point(141, 13);
            this.comboWaveInDevice.Name = "comboWaveInDevice";
            this.comboWaveInDevice.Size = new System.Drawing.Size(121, 21);
            this.comboWaveInDevice.TabIndex = 12;
            // 
            // comboWasapiDevices
            // 
            this.comboWasapiDevices.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboWasapiDevices.FormattingEnabled = true;
            this.comboWasapiDevices.Location = new System.Drawing.Point(141, 116);
            this.comboWasapiDevices.Name = "comboWasapiDevices";
            this.comboWasapiDevices.Size = new System.Drawing.Size(121, 21);
            this.comboWasapiDevices.TabIndex = 12;
            // 
            // radioButtonWasapiLoopback
            // 
            this.radioButtonWasapiLoopback.AutoSize = true;
            this.radioButtonWasapiLoopback.Location = new System.Drawing.Point(6, 143);
            this.radioButtonWasapiLoopback.Name = "radioButtonWasapiLoopback";
            this.radioButtonWasapiLoopback.Size = new System.Drawing.Size(118, 17);
            this.radioButtonWasapiLoopback.TabIndex = 8;
            this.radioButtonWasapiLoopback.Text = "WASAPI Loopback";
            this.radioButtonWasapiLoopback.UseVisualStyleBackColor = true;
            // 
            // radioButtonWasapi
            // 
            this.radioButtonWasapi.AutoSize = true;
            this.radioButtonWasapi.Location = new System.Drawing.Point(6, 120);
            this.radioButtonWasapi.Name = "radioButtonWasapi";
            this.radioButtonWasapi.Size = new System.Drawing.Size(67, 17);
            this.radioButtonWasapi.TabIndex = 9;
            this.radioButtonWasapi.Text = "WASAPI";
            this.radioButtonWasapi.UseVisualStyleBackColor = true;
            // 
            // radioButtonWaveIn
            // 
            this.radioButtonWaveIn.AutoSize = true;
            this.radioButtonWaveIn.Checked = true;
            this.radioButtonWaveIn.Location = new System.Drawing.Point(6, 17);
            this.radioButtonWaveIn.Name = "radioButtonWaveIn";
            this.radioButtonWaveIn.Size = new System.Drawing.Size(60, 17);
            this.radioButtonWaveIn.TabIndex = 11;
            this.radioButtonWaveIn.TabStop = true;
            this.radioButtonWaveIn.Text = "waveIn";
            this.radioButtonWaveIn.UseVisualStyleBackColor = true;
            // 
            // comboWasapiLoopbackDevices
            // 
            this.comboWasapiLoopbackDevices.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboWasapiLoopbackDevices.FormattingEnabled = true;
            this.comboWasapiLoopbackDevices.Location = new System.Drawing.Point(141, 145);
            this.comboWasapiLoopbackDevices.Name = "comboWasapiLoopbackDevices";
            this.comboWasapiLoopbackDevices.Size = new System.Drawing.Size(121, 21);
            this.comboWasapiLoopbackDevices.TabIndex = 14;
            // 
            // RecordingPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBoxRecordingApi);
            this.Controls.Add(this.buttonOpenFolder);
            this.Controls.Add(this.buttonDelete);
            this.Controls.Add(this.buttonPlay);
            this.Controls.Add(this.listBoxRecordings);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonStopRecording);
            this.Controls.Add(this.buttonStartRecording);
            this.Name = "RecordingPanel";
            this.Size = new System.Drawing.Size(614, 371);
            this.groupBoxRecordingApi.ResumeLayout(false);
            this.groupBoxRecordingApi.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonStartRecording;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonStopRecording;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ListBox listBoxRecordings;
        private System.Windows.Forms.Button buttonPlay;
        private System.Windows.Forms.Button buttonDelete;
        private System.Windows.Forms.Button buttonOpenFolder;
        private System.Windows.Forms.GroupBox groupBoxRecordingApi;
        private System.Windows.Forms.ComboBox comboWasapiDevices;
        private System.Windows.Forms.RadioButton radioButtonWasapiLoopback;
        private System.Windows.Forms.RadioButton radioButtonWasapi;
        private System.Windows.Forms.RadioButton radioButtonWaveIn;
        private System.Windows.Forms.ComboBox comboWaveInDevice;
        private System.Windows.Forms.CheckBox checkBoxEventCallback;
        private System.Windows.Forms.ComboBox comboBoxChannels;
        private System.Windows.Forms.ComboBox comboBoxSampleRate;
        private System.Windows.Forms.ComboBox comboWasapiLoopbackDevices;
    }
}
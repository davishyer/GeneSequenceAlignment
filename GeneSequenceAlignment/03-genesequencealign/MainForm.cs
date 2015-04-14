
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace GeneticsLab
{
    public partial class MainForm : Form
    {
        DatabaseController dbController;
        ResultTable resultTable;
        GeneSequence[] sequences;
        PairWiseAlign processor;

        public MainForm()
        {
            InitializeComponent();
            this.dbController = new DatabaseController();
            this.dbController.EstablishConnection("../../db1.mdb");
            statusMessage.Text = "Loading Database...";
            // Set the number of Sequences to load below.
            this.sequences = this.dbController.ReadGeneSequences(10);
            this.resultTable = new ResultTable(this.dataGridViewResults, this.sequences.Length);
            statusMessage.Text = "Loaded Database.";
            processor = new PairWiseAlign();
        }

        private void fillMatrix()
        {
            for (int y = 0; y < this.sequences.Length; ++y)
                for (int x = 0; x < this.sequences.Length; ++x)
                {
                    if (x <= y)
                        this.resultTable.SetCell(x, y, "-");
                    else
                        this.resultTable.SetCell(x, y, processor.Align(this.sequences[x], this.sequences[y],this.resultTable,x,y));
                }
        }

        private void processButton_Click(object sender, EventArgs e)
        {
            statusMessage.Text = "Processing...";
            Stopwatch timer = new Stopwatch();
            timer.Start();
            fillMatrix();
            timer.Stop();
            statusMessage.Text = "Done.  Time taken: " + timer.Elapsed;

        }

        private void dataGridViewResults_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            GeneSequence seqA = this.sequences[e.ColumnIndex];
            GeneSequence seqB = this.sequences[e.RowIndex];

            String[] results = processor.extraction(seqA, seqB);

            String outputText = "Output Console:";
            outputText += "\r\nGene Alignment for Cell (";
            outputText += (e.RowIndex + 1) + ", ";
            outputText += (e.ColumnIndex + 1) + ")";
            outputText += "\r\nA: " + processor.stringLimit(results[0], 100);
            outputText += "\r\nB: " + processor.stringLimit(results[1], 100);

            outputConsole.Text = outputText;
        }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Soduko
{
    public partial class Form1 : Form
    {
        static int[,] Defaults = {
                                    { 7, 0, 6, 2, 0, 4, 0, 0, 0 },
                                    { 0, 0, 0, 8, 0, 0, 3, 0, 0 },
                                    { 5, 0, 0, 1, 0, 0, 0, 4, 0 },
                                    { 9, 8, 7, 0, 4, 2, 0, 0, 5 },
                                    { 0, 0, 0, 3, 0, 8, 0, 0, 0 },
                                    { 3, 0, 0, 7, 1, 0, 2, 8, 6 },
                                    { 0, 9, 0, 0, 0, 6, 0, 0, 8 },
                                    { 0, 0, 2, 0, 0, 7, 0, 0, 0 },
                                    { 0, 0, 0, 5, 0, 1, 9, 0, 2 }
                                };

        public Form1()
        {
            InitializeComponent();
        }

        //================================================================================

        private void Form1_Load(object sender, EventArgs e)
        {
            for (int i = 0; i < 9; i++)
                dataGridView1.RowCount = 9;
            dataGridView1.Rows[0].Cells[0].ValueType = typeof(int);
            for (int i = 0; i < 9; i++)
                for (int j = 0; j < 9; j++)
                {
                    dataGridView1.Rows[i].Cells[j].ValueType = typeof(int);
                    if (Defaults[i, j] != 0)
                        dataGridView1.Rows[i].Cells[j].Value = Defaults[i, j];
                }
        }

        //================================================================================

        internal int CellValue(
                int iCol,
                int iRow)
        {
            var cell = dataGridView1.Rows[iRow].Cells[iCol];
            if (cell.Value == null)
                return 0;
            return (int)cell.Value;
        }

        //================================================================================

        bool NextCell(
                    int iCol,
                    int iRow)
        {
            int iColNext = (iCol + 1) % 9;
            int iRowNext = iRow;
            if (iColNext == 0)
                iRowNext++;
            return FindNumberForThisCell(iColNext, iRowNext);
        }

        //================================================================================

        internal bool FindNumberForThisCell(
                int iCol,
                int iRow)
        {
            if (iRow == 9)
            {
                dataGridView1.Update();
                MessageBox.Show("This is a solution");
                return false;
            }

            int iCellValue = CellValue(iCol, iRow);      
            bool bOK = iCellValue != 0;                 // already has a valid value

            if (bOK)
            {
                if (CellCanHaveThisValue(iCol, iRow, iCellValue))
                    bOK = NextCell(iCol, iRow);
                else
                    throw new Exception(string.Format("Given number {0} at {1}/{2} is not possible", iCellValue, iCol, iRow));
            }
            else
            {
                for (int i = 1; i <= 9; i++)
                {
                    if (CellCanHaveThisValue(iCol, iRow, i))
                    {
                        dataGridView1.Rows[iRow].Cells[iCol].Value = i;
                        bOK = NextCell(iCol, iRow);
                        if (!bOK)
                            dataGridView1.Rows[iRow].Cells[iCol].Value = null;
                        dataGridView1.Update();
                    }
                }
            }
            return bOK;
        }

        //================================================================================

        private bool CellCanHaveThisValue(int iCol, int iRow, int x)
        {
            bool bOK = true;

            // is it already in this row?
            for (int i = 0; i < 9 && bOK; i++)
                if (i != iCol && CellValue(i, iRow) == x)
                    bOK = false;

            // is it already in this column?
            for (int i = 0; i < 9 && bOK; i++)
                if (i != iRow && CellValue(iCol, i) == x)
                    bOK = false;

            if (bOK)
            {
                int iColBlock = (iCol / 3) * 3;
                int iRowBlock = (iRow / 3) * 3;
                for (int i = 0; i < 3; i++)
                    for (int j = 0; j < 3; j++)
                        if (iColBlock + i != iCol && iRowBlock + j != iRow && CellValue(iColBlock + i, iRowBlock + j) == x)
                            bOK = false;
            }
            return bOK;
        }

        //==============================================================================================

        private void Solve(object sender, EventArgs e)
        {
            for (int i = 0; i < 9; i++)
                for (int j = 0; j < 9; j++)
                {
                    if (CellValue(j, i) == 0)
                    {
                        dataGridView1.Rows[i].Cells[j].Value = null;
                        dataGridView1.Rows[i].Cells[j].Style.ForeColor = Color.Black;
                    }
                    else
                        dataGridView1.Rows[i].Cells[j].Style.ForeColor = Color.Red;
                    dataGridView1.Rows[i].Cells[j].Selected = false;
                }

            dataGridView1.Update();

            try 
            {
                FindNumberForThisCell(0, 0);
                dataGridView1.Update();
                MessageBox.Show("No more solutions found");
            }
            catch (Exception ex)
            {
                dataGridView1.Update();
                MessageBox.Show(ex.Message);
            }
        }

        //==============================================================================================

        private void buttonClear_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 9; i++)
                for (int j = 0; j < 9; j++)
                {
                    dataGridView1.Rows[i].Cells[j].Value = null;
                    dataGridView1.Rows[i].Cells[j].Style.ForeColor = Color.Black;
                }
            dataGridView1.Update();
        }

        //==============================================================================================

    }
}

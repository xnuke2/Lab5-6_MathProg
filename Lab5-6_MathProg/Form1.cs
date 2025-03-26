using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab5_6_MathProg
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            dataGridViewConditions.Rows.Clear();
            dataGridViewConditions.Columns.Clear();

            int rows =Convert.ToInt32(numericUpDown1.Value);
            int columns =Convert.ToInt32(numericUpDown2.Value);

            for (int i = 0; i < columns + 1; i++)
                dataGridViewConditions.Columns.Add(""+i, "" + i);
            for (int i = 0; i < rows + 1; i++)
                dataGridViewConditions.Rows.Add();
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            dataGridViewCellStyle1.BackColor = Color.Cyan;
            dataGridViewCellStyle1.ForeColor = SystemColors.WindowText;
            for (int i = 0;i < columns + 1; i++)
                dataGridViewConditions[i,0].Style = dataGridViewCellStyle1;
            for(int i = 0;i<rows + 1; i++)
                dataGridViewConditions[0,i].Style = dataGridViewCellStyle1;
            for (int i = 0; i < columns + 1; i++)
                for (int j = 0; j < rows + 1; j++)
                    dataGridViewConditions[i, j].Value = 0;
            dataGridViewConditions[0, 0].Value = "";
            dataGridViewConditions[0, 0].ReadOnly = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            numericUpDown1_ValueChanged(sender, e);
        }

        private void buttonCulculate_Click(object sender, EventArgs e)
        {
            int rowsCount = dataGridViewConditions.RowCount-1;
            int columnsCount = dataGridViewConditions.ColumnCount-1;
            int[] columns =new int[columnsCount];
            int[] rows = new int[rowsCount];
            int[,] costs=new int[rowsCount, columnsCount];

            for(int i = 0;i< rowsCount;i++)
                rows[i] = Convert.ToInt32(dataGridViewConditions[0,i+1].Value);

            for(int i = 0;i< columnsCount;i++)
                columns[i] = Convert.ToInt32(dataGridViewConditions[i+1,0].Value);

            for (int i = 0; i< rowsCount;i++)
                for(int j = 0;j< columnsCount;j++)
                    costs[i,j] =Convert.ToInt32(dataGridViewConditions[j+1,i+1].Value);

            //При необходимости перевод открытой модели к закрытой
            if (rows.Sum() != columns.Sum())
                if (rows.Sum() > columns.Sum())
                {
                    int[] tmpColumns = new int[columnsCount + 1];
                    for (int i = 0; i < columnsCount; i++)
                        tmpColumns[i] = columns[i];
                    columns = tmpColumns;
                    tmpColumns[columnsCount++] = rows.Sum() - columns.Sum();
                    int[,] tmpCosts = new int[rowsCount, columnsCount];
                    for (int i = 0;i< rowsCount;i++)
                        for (int j = 0; j< columnsCount-1;j++)
                            tmpCosts[i,j]=costs[i,j];
                    costs = tmpCosts;
                }
                else
                {
                    int[] tmpRows = new int[rowsCount + 1];
                    for (int i = 0; i < rowsCount; i++)
                        tmpRows[i] = rows[i];
                    rows = tmpRows;
                    tmpRows[rowsCount++] = columns.Sum()  - rows.Sum();
                    int[,] tmpCosts = new int[rowsCount, columnsCount];
                    for (int i = 0; i < rowsCount-1; i++)
                        for (int j = 0; j < columnsCount; j++)
                            tmpCosts[i, j] = costs[i, j];
                    costs = tmpCosts;
                }
            int?[,] counts = new int?[rowsCount, columnsCount];
            int rowIndex = 0;
            int columnsIndex = 0;

            int[] columnsTmp =new int[columnsCount];
            columns.CopyTo(columnsTmp, 0);
            int[] rowsTmp =new int[rowsCount];
            rows.CopyTo(rowsTmp, 0);

            List<Point> basis = new List<Point>();

            while (rowIndex < rowsCount && columnsIndex < columnsCount)
            {
                basis.Add(new Point(rowIndex, columnsIndex));
                if (columnsTmp[columnsIndex] > rowsTmp[rowIndex])
                { 
                    counts[rowIndex, columnsIndex] = rowsTmp[rowIndex];
                    columnsTmp[columnsIndex] -= rowsTmp[rowIndex];
                    rowsTmp[rowIndex]=0;
                    rowIndex++;
                }
                else if(columnsTmp[columnsIndex] < rowsTmp[rowIndex])
                {
                    counts[rowIndex, columnsIndex] = columnsTmp[columnsIndex];
                    rowsTmp[rowIndex] -= columnsTmp[columnsIndex];
                    columnsTmp[columnsIndex] = 0;
                    columnsIndex++;
                }  
                else 
                {
                    if (rowIndex == rowsCount - 1 && columnsIndex == columnsCount - 1)
                    {
                        counts[rowIndex, columnsIndex] = rowsTmp[rowIndex];
                        rowsTmp[rowIndex] = 0;
                        columnsTmp[columnsIndex] = 0;
                        rowIndex++;
                    }
                    else
                    {
                        if(rowIndex == rowsCount - 1)
                        {
                            counts[rowIndex, columnsIndex] = rowsTmp[rowIndex];
                            rowsTmp[rowIndex] = 0;
                            columnsTmp[columnsIndex] = 0;
                            columnsIndex++;
                            counts[rowIndex, columnsIndex] = 0;
                            basis.Add(new Point(rowIndex, columnsIndex));
                            rowIndex++;
                        }
                        else
                        {
                            counts[rowIndex, columnsIndex] = rowsTmp[rowIndex];
                            rowsTmp[rowIndex] = 0;
                            columnsTmp[columnsIndex] = 0;
                            rowIndex++;
                            counts[rowIndex, columnsIndex] = 0;
                            basis.Add(new Point(rowIndex, columnsIndex));
                            columnsIndex++;
                        }
                    }

                }

            }
            dataGridViewSolutions.Rows.Clear();
            dataGridViewSolutions.Columns.Clear();
            for (int i = 0; i < columns.Length + 1; i++)
                dataGridViewSolutions.Columns.Add("" + i, "" + i);
            for (int i = 0; i < rows.Length + 1; i++)
                dataGridViewSolutions.Rows.Add();
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            dataGridViewCellStyle1.BackColor = Color.Cyan;
            dataGridViewCellStyle1.ForeColor = SystemColors.WindowText;
            for (int i = 0; i < columns.Length + 1; i++)
                dataGridViewSolutions[i, 0].Style = dataGridViewCellStyle1;
            for (int i = 0; i < rows.Length + 1; i++)
                dataGridViewSolutions[0, i].Style = dataGridViewCellStyle1;
            for (int i = 1; i < columns.Length + 1; i++)
                for (int j = 1; j < rows.Length + 1; j++)
                    dataGridViewSolutions[i, j].Value = "-";

            for (int i = 1; i < columns.Length + 1; i++)
                for (int j = 1; j < rows.Length + 1; j++)
                    if(counts[j - 1, i - 1]!=null)
                        dataGridViewSolutions[i, j].Value = counts[j-1,i-1];

            for (int i = 1; i < columns.Length + 1; i++)
                dataGridViewSolutions[i,0].Value = columns[i - 1];
            for (int j = 1; j < rows.Length + 1; j++)
                dataGridViewSolutions[0, j].Value = rows[j-1];

                dataGridViewSolutions[0, 0].Value = "";
            dataGridViewSolutions[0, 0].ReadOnly = true;
            
            dataGridViewResult.Rows.Clear();
            dataGridViewResult.Columns.Clear();
            dataGridViewResult.Columns.Add("Z","Z");
            dataGridViewResult.Columns.Add("Atributes", "Свойсва");
            string atributes = "";
            for (int i = 0; i < columnsCount; i++)
            {
                int sum = 0;
                for (int j = 0; j < rowsCount; j++)
                    if(counts[j, i]!=null)
                        sum += (int)counts[j, i];
                if(sum!=columns[i])
                {
                    atributes += "недопустимая\n";
                    break;
                }
            }
            for (int i = 0; i < rowsCount; i++)
            {
                int sum = 0;
                for (int j = 0; j <columnsCount; j++)
                    if (counts[ i,j] != null)
                        sum += (int)counts[i, j];
                if (sum != rows[i])
                {
                    if(!atributes.Contains("недопустимая"))
                        atributes += "недопустимая";
                    break;
                }
            }
            if (!atributes.Contains("недопустимая"))
                atributes += "допустимая";
            atributes += " \n";
            if (columns.Length+rows.Length-1==basis.Count)
                atributes += "базисное";
            else atributes += "небазисное";
            atributes += " \n";
            for (int i = 0; i < basis.Count; i++)
                if (counts[basis[i].X, basis[i].Y] != null && counts[basis[i].X, basis[i].Y] < 0)
                    atributes += "вырожденное";
            if(!atributes.Contains("вырожденное"))
                atributes += "невырожденное";
            int initZ = 0;
            for (int i = 0; i < basis.Count; i++)
                initZ += (int)counts[basis[i].X, basis[i].Y] * costs[basis[i].X, basis[i].Y];
            dataGridViewResult.Rows.Add(initZ,atributes);
        }
    }
}

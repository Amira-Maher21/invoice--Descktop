using DevExpress.XtraEditors;
using Invoice1.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Invoice1
{
    public partial class Form1 : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        DataClassesDataContext DB = new DataClassesDataContext();


        public bool ISEdite;
        public DataTable Invoicedata;
        private decimal price2;
        private object price;

        public int Quantity { get; private set; }
        public int ItemId { get; private set; }

        public Form1()
        {
            InitializeComponent();

           // CreateGrid();
        }
        void InsertOrUpdate()
        {
      
          try
            {

                InvoiceMaster ma = new InvoiceMaster();

                ma.DocTypeId = Convert.ToInt32(searchLookUpEditDocType.EditValue);
                //ma.InvoiceDate = Convert.ToDateTime(dateEditInvoicee.Text);
                ma.StoreId = Convert.ToInt32(searchLookUpEditStore.EditValue);

                ma.MainDiscount = Convert.ToDecimal(MainDiscount.EditValue);
                ma.LastFinalTotalDiscount = Convert.ToDecimal(LastFinalTotalDiscount.EditValue);
                ma.FinalInvoiceWinValue = Convert.ToDecimal(FinalInvoiceWinValue.EditValue);
                ma.InvoiceWinValue = Convert.ToDecimal(InvoiceWinValue.EditValue);
                ma.InvoiceNetValue2 = Convert.ToDecimal(InvoiceNetValue2.EditValue);
                ma.InvoiceNetValueAfterDiscount = Convert.ToDecimal(InvoiceNetValueAfterDiscount.EditValue);

                DB.InvoiceMasters.InsertOnSubmit(ma);

                DB.SubmitChanges();



                for (int i = 0; i < gridView1.RowCount; i++)
                {

                    int id = Convert.ToInt32(gridView1.GetRowCellValue(i, "BookId"));
                    int qty = Convert.ToInt32(gridView1.GetRowCellValue(i, "BookQuantity"));
                    decimal pri = Convert.ToDecimal(gridView1.GetRowCellValue(i, "BookPrice"));
                    decimal tot = Convert.ToDecimal(gridView1.GetRowCellValue(i, "Total"));

                    DB.InsertInDetail(ma.InvoiceId, id, qty, pri, tot);
                    DB.SubmitChanges();
                }

                MessageBox.Show("تم الحفظ");
            }
            catch
            {

                MessageBox.Show("لم يتم الحفظ");
            }


        }


        private void CreateGrid()
        {
            Invoicedata = new DataTable();

            Invoicedata.Columns.Add("ItemId", typeof(int));
            Invoicedata.Columns.Add("Quantity", typeof(decimal));
            Invoicedata.Columns.Add("Itemsllprice", typeof(decimal));
            Invoicedata.Columns.Add("Total", typeof(decimal)).Expression = "BookQuantity*BookPrice";
            gridControl1.DataSource = Invoicedata;
        }


        private void Fill()
        {

            dateEditInvoicee.EditValue = DateTime.Now;
            searchLookUpEditDocType.Properties.DataSource = DB.DocTypes.ToList();
            searchLookUpEditDocType.EditValue = 1;
            searchLookUpEditStore.Properties.DataSource = DB.Stores.ToList();


            repositoryItemLookUpEdit1.DataSource = DB.items.ToList();

        }

        private void Claculation()
        {

            decimal tot = new decimal();
            decimal netval = new decimal();
            decimal dis = new decimal();
            decimal maintot = new decimal();

            for (int i = 0; i < gridView1.RowCount; i++)
            {
                gridView1.FocusedRowHandle = -1;
                tot = Convert.ToDecimal(gridView1.GetRowCellValue(i, "Total"));
                netval = netval + tot;

            }

            dis = Convert.ToDecimal(MainDiscount.EditValue);
            if (dis == 0)
            {
                InvoiceNetValueAfterDiscount.EditValue = Convert.ToDecimal(netval);
                InvoiceNetValue2.EditValue = Convert.ToDecimal(netval);
            }
            else
            {
                maintot = netval - dis;
                InvoiceNetValueAfterDiscount.EditValue = Convert.ToDecimal(netval);
                InvoiceNetValue2.EditValue = 0;
                InvoiceNetValue2.EditValue = Convert.ToDecimal(maintot);
            }

        }




        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            InsertOrUpdate();

        }

        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.Close();
        }

        private void ribbonControl1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Fill();
        }

        private void gridControl1_KeyUp(object sender, KeyEventArgs e)
        {
            {
                Claculation();
            }
            int ItemId;
            decimal price;
            decimal price2;
            decimal Quantity;
        }

        private void gridControl1_KeyDown(object sender, KeyEventArgs e)
        {


            string Barcode = bookbarcode.Text;
            int docid = Convert.ToInt32(searchLookUpEditDocType.EditValue);
            if (e.KeyCode == Keys.Enter)
            {

                if (Barcode != null || Barcode != string.Empty || Barcode != "")
                {
                    var data = DB.items.Where(a => a.Barcode == Barcode).FirstOrDefault();
                    if (data != null)
                    {
                        ItemId = Convert.ToInt32(data.ItemId);
                        price = Convert.ToDecimal(data.itemsllprice);
                        price2 = Convert.ToDecimal(data.itembuypirice);
                        Quantity = 1;

                        if (docid == 1 || docid == 2 || docid == 5 || docid == 6)
                        {

                            Invoicedata.Rows.Add(ItemId, price, Quantity);
                            bookbarcode.Text = "";
                            bookbarcode.Focus();
                            Claculation();
                        }
                        else


                        {

                            Invoicedata.Rows.Add(ItemId, price2, Quantity);
                            bookbarcode.Text = "";
                            bookbarcode.Focus();
                            Claculation();
                        }

                    }
                    else
                    {
                        XtraMessageBox.Show("لايوجد صنف بهذا الرقم  الباركود خاطئ");
                        bookbarcode.Text = null;
                        bookbarcode.Focus();


                    }

                }
            }
        }


        private void barButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }



        private void gridControl1_Click(object sender, EventArgs e)
        {

     
     
            }

        private void gridView1_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            int id = new int();
            int docid = Convert.ToInt32(searchLookUpEditDocType.EditValue);

            if (e.Column.FieldName == "ItemId")
            {
                id = Convert.ToInt32(gridView1.GetRowCellValue(e.RowHandle, "ItemId"));

                var data = DB.items.Where(a => a.ItemId == id).FirstOrDefault();

                gridView1.SetRowCellValue(e.RowHandle, "Quantity", 1);

                if (docid == 1 || docid == 2 || docid == 5 || docid == 6)
                {
                    gridView1.SetRowCellValue(e.RowHandle, "itemsllprice", data.itemsllprice);
                }
                else
                {
                    gridView1.SetRowCellValue(e.RowHandle, "itembuypirice", data.itembuypirice);
                }

                gridView1.FocusedRowHandle = -1;
                Claculation();
            }
        }

        private void barButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }
    }
    }
  


/*
* 
* Created by Akshay Srinivasan
* TCS Microsoft CoE
* Manager Girish Phadke
* Copyright Tata Consultancy Services (TCS) 2012
* This is proprietary code and is not meant for use outside of TCS projects.
* Code is meant for TCS internal use only and should be used with the consultation of TCS Microsoft CoE.
* 
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Security.Permissions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TCS.Web.Charting.Tools
{
    [AspNetHostingPermission(SecurityAction.Demand,
       Level = AspNetHostingPermissionLevel.Minimal)]
    [AspNetHostingPermission(SecurityAction.InheritanceDemand,
        Level = AspNetHostingPermissionLevel.Minimal)]
    public class LineAreaChart : DataBoundControl
    {
        private string dataJSString;
        private bool Labeled = false;
        public string Title { get; set; }
        public List<string> ColorsHexStrings { get; set; }
        public int XMaxValue { get; set; }
        public int YMaxValue { get; set; }
        public int NumMarksX { get; set; }
        public int NumMarksY { get; set; }
        public int XMarksWidth { get; set; }
        
        protected override void PerformSelect()
        {
            if (!IsBoundUsingDataSourceID)
            {
                this.OnDataBinding(EventArgs.Empty);
            }
            GetData().Select(CreateDataSourceSelectArguments(),
                this.OnDataSourceViewSelectCallback);
            RequiresDataBinding = false;
            MarkAsDataBound();
            OnDataBound(EventArgs.Empty);
        }

        private void OnDataSourceViewSelectCallback(IEnumerable retrievedData)
        {
            if (IsBoundUsingDataSourceID)
            {
                OnDataBinding(EventArgs.Empty);
            }
            PerformDataBinding(retrievedData);
        }

        protected override void PerformDataBinding(IEnumerable retrievedData)
        {
            base.PerformDataBinding(retrievedData);
            dataJSString = "[[";
            if (retrievedData != null)
            {
                foreach (object dataItem in retrievedData)
                {
                    dataJSString += "[";
                    PropertyDescriptorCollection props =
                            TypeDescriptor.GetProperties(dataItem);
                    for (int x = 0; x < props.Count; x++)
                    {
                        if (null != props[x].GetValue(dataItem))
                        {
                            if (x == 0 || (x != 1 && x < props.Count - 1))
                            {
                                if (IsNumber(props[x].GetValue(dataItem).ToString()))
                                {
                                    dataJSString += props[x].GetValue(dataItem).ToString() + ",";
                                }
                                else
                                {
                                    dataJSString += "'" + props[x].GetValue(dataItem).ToString() + "',";
                                    Labeled = true;
                                }
                            }
                            else if (x == 1)
                            {
                                if (IsNumber(props[x].GetValue(dataItem).ToString()))
                                {
                                    dataJSString += "[" + props[x].GetValue(dataItem).ToString() + ",";
                                }
                                else
                                {
                                    dataJSString += "['" + props[x].GetValue(dataItem).ToString() + "',";
                                }
                            }
                            else if (x == props.Count - 1)
                            {
                                if (IsNumber(props[x].GetValue(dataItem).ToString()))
                                {
                                    dataJSString += props[x].GetValue(dataItem).ToString() + "]]";
                                }
                                else
                                {
                                    dataJSString += "'" + props[x].GetValue(dataItem).ToString() + "']]";
                                }
                            }
                        }
                    }
                    dataJSString = dataJSString.Substring(0, dataJSString.Length - 1) + "],";
                }
            }
            dataJSString = dataJSString.Substring(0, dataJSString.Length - 1) + "],[";
            for (int x = 0; x < ColorsHexStrings.Count; x++)
            {
                dataJSString += "'" + ColorsHexStrings[x] + "',";
            }
            dataJSString = dataJSString.Substring(0, dataJSString.Length - 1) + "]]";
        }

        protected override void Render(HtmlTextWriter writer)
        {
            writer.Write("<canvas id=\"" + this.ID.ToString() + "\" width=\"" + this.Width.ToString() + "\" height=\"" + this.Height.ToString() +
                "\"><script language=\"javascript\" type=\"text/javascript\">" + (Labeled ? "drawLabeledLineAreaGraph" : "drawLineAreaGraph") +
                "('" + this.ID.ToString() + "', " + dataJSString + "," + (Labeled ? "" : XMaxValue.ToString() + ",") + YMaxValue.ToString() + "," + 
                (Labeled ? "" : NumMarksX.ToString() + ",") + NumMarksY.ToString() + ",'" + this.Title.ToString() + "'" + 
                (Labeled ? "," + XMarksWidth.ToString() : "") + ");</script></canvas>");
        }

        private bool IsNumber(string str)
        {
            double Num;
            return double.TryParse(str, out Num);
        }
    }
}

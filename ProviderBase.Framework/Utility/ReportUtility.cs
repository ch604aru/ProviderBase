using ProviderBase.Data.Entities;
using ProviderBase.Framework.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace ProviderBase.Framework
{
    public class ReportUtility
    {
        private Report Report { get; set; }

        private Website Website { get; set; }

        public ReportUtility(Website website, int reportID)
        {
            this.Website = website;
            this.Report = ProviderBase.Data.Providers.DataProvider.SelectSingle<Report>(new Report()
            {
                ReportID = reportID
            }, website.WebsiteConnection.ConnectionString);
        }

        public ReportUtility(Website website, string reportGUID)
        {
            this.Website = website;
            this.Report = ProviderBase.Data.Providers.DataProvider.SelectSingle<Report>(new Report()
            {
                GUID = new Guid(reportGUID)
            }, website.WebsiteConnection.ConnectionString);
        }

        public ReportUtility(Website website, Report report)
        {
            this.Website = website;
            this.Report = report;
        }

        public string Render()
        {
            string reportTemplate = "";

            if (this.Report?.ReportID > 0)
            {
                List<ReportField> reportFieldList = null;

                reportFieldList = ProviderBase.Data.Providers.DataProvider.Select<ReportField>(new ReportField()
                {
                    ReportID = this.Report.ReportID
                }, this.Website.WebsiteConnection.ConnectionString);

                if (reportFieldList?.Count > 0)
                {
                    string reportTitleStartTag = "";
                    string reportTitleEndTag = "";
                    string reportStartTag = "";
                    string reportEndTag = "";
                    string reportHeaderStartTag = "";
                    string reportHeaderEndTag = "";
                    string reportRowStartTag = "";
                    string reportRowEndTag = "";

                    this.DrawElementTitle(this.Report, out reportTitleStartTag, out reportTitleEndTag);
                    reportTemplate += reportTitleStartTag;
                    reportTemplate += reportTitleEndTag;

                    this.DrawElementReport(this.Report, out reportStartTag, out reportEndTag);
                    reportTemplate += reportStartTag;

                    this.DrawElementHeader(this.Report, out reportHeaderStartTag, out reportHeaderEndTag);
                    reportTemplate += reportHeaderStartTag;

                    // Header
                    foreach (ReportField reportField in reportFieldList)
                    {
                        string reportFieldHeaderContainerStartTag = "";
                        string reportFieldHeaderContainerEndTag = "";

                        this.DrawElementHeaderContainer(this.Report, reportField, out reportFieldHeaderContainerStartTag, out reportFieldHeaderContainerEndTag);
                        reportTemplate += reportFieldHeaderContainerStartTag;
                        reportTemplate += reportFieldHeaderContainerEndTag;
                    }

                    reportTemplate += reportHeaderEndTag;

                    // Field
                    reportTemplate += "<repeat>";

                    this.DrawElementRow(this.Report, out reportRowStartTag, out reportRowEndTag);
                    reportTemplate += reportRowStartTag;

                    foreach (ReportField reportField in reportFieldList)
                    {
                        string reportFieldRowContainerStartTag = "";
                        string reportFieldRowContainerEndTag = "";
                        string reportFieldRowItemStartTag = "";
                        string reportFieldRowItemEndTag = "";

                        this.DrawElementRowContainer(this.Report, reportField, out reportFieldRowContainerStartTag, out reportFieldRowContainerEndTag);
                        reportTemplate += reportFieldRowContainerStartTag;

                        this.DrawElementRowItem(this.Report, reportField, out reportFieldRowItemStartTag, out reportFieldRowItemEndTag);
                        reportTemplate += reportFieldRowItemStartTag;
                        reportTemplate += reportFieldRowItemEndTag;

                        reportTemplate += reportFieldRowContainerEndTag;
                    }

                    reportTemplate += reportRowEndTag;
                    reportTemplate += "</repeat>";

                    reportTemplate += reportEndTag;
                }
                else
                {
                    reportTemplate = "Unable to load report fields";
                }
            }
            else
            {
                reportTemplate = "Unable to load report";
            }

            return reportTemplate;
        }

        public string Render(object bindObject)
        {
            string template = "";
            string templateRepeat = "";

            template = this.Render();
            templateRepeat = ProviderBase.Data.Utility.GetTemplateFileElementSingle(template, "repeat", "repeat");
            template = ProviderBase.Data.Utility.ReplaceTemplateFileElementSingle(template, "repeat", "repeat");

            if (ProviderBase.Data.Utility.IsList(bindObject))
            {
                IList bindObjectList = (IList)bindObject;
                string templateRepeatTemp = "";

                foreach (object bindObjectItem in bindObjectList)
                {
                    templateRepeatTemp += ProviderBase.Data.Utility.TemplateBindData(templateRepeat, bindObjectItem);
                }

                template = template.Replace("$repeat$", templateRepeatTemp);
            }
            else
            {
                templateRepeat = ProviderBase.Data.Utility.TemplateBindData(templateRepeat, bindObject);
                template = template.Replace("$repeat$", templateRepeat);
            }

            return template;
        }

        private void DrawElementTitle(Report report, out string startTag, out string endTag)
        {
            startTag = "";
            endTag = "";

            startTag += "<div";
            startTag += (string.IsNullOrEmpty(report.Class)) ? "" : $" class=\"{report.Class}-title\"";
            startTag += ">";
            startTag += report.Title;

            endTag += "</div>";
        }

        private void DrawElementReport(Report report, out string startTag, out string endTag)
        {
            startTag = "";
            endTag = "";

            startTag += "<div";
            startTag += (string.IsNullOrEmpty(report.Class)) ? "" : $" class=\"{report.Class}\"";
            startTag += (string.IsNullOrEmpty(report.Style)) ? "" : $" style=\"{report.Style}\"";
            startTag += ">";

            endTag += "</div>";
        }

        private void DrawElementHeader(Report report, out string startTag, out string endTag)
        {
            startTag = "";
            endTag = "";

            startTag += "<div";
            startTag += (string.IsNullOrEmpty(report.Class)) ? "" : $" class=\"{report.Class}-header\"";
            startTag += ">";

            endTag += "</div>";
        }

        private void DrawElementHeaderContainer(Report report, ReportField reportField, out string startTag, out string endTag)
        {
            startTag = "";
            endTag = "";

            startTag += "<div";
            startTag += (string.IsNullOrEmpty(report.Class)) ? "" : $" class=\"{report.Class}-header-container";
            startTag += (reportField.ContainerWidth > 0) ? $" width{reportField.ContainerWidth}\"" : "\"";
            startTag += ">";
            startTag += reportField.Title;

            endTag += "</div>";
        }

        private void DrawElementRow(Report report, out string startTag, out string endTag)
        {
            startTag = "";
            endTag = "";

            startTag = "<div";
            startTag += (string.IsNullOrEmpty(report.Class)) ? "" : $" class=\"{report.Class}-row\"";
            startTag += ">";

            endTag += "</div>";
        }

        private void DrawElementRowContainer(Report report, ReportField reportField, out string startTag, out string endTag)
        {
            startTag = "";
            endTag = "";

            startTag += "<div";
            startTag += (string.IsNullOrEmpty(report.Class)) ? "" : $" class=\"{report.Class}-row-container";
            startTag += (reportField.ContainerWidth > 0) ? $" width{reportField.ContainerWidth}\"" : "\"";
            startTag += ">";

            endTag += "</div>";
        }

        private void DrawElementRowItem(Report report, ReportField reportField, out string startTag, out string endTag)
        {
            startTag = "";
            endTag = "";

            if (reportField.ReportFIeldEventTypeID == ReportFIeldEventType.a)
            {
                startTag += $"<a href=\"{reportField.FieldEvent}${reportField.FieldName.ToUpper()}$\">";
                startTag += "<div";
                startTag += (string.IsNullOrEmpty(reportField.Class)) ? "" : $" class=\"{reportField.Class}\"";
                startTag += ">";

                endTag += "</div>";
                endTag += "</a>";
            }
            else
            {
                startTag += "<span";
                startTag += (string.IsNullOrEmpty(report.Class)) ? "" : $" class=\"{report.Class}-row-item\"";
                startTag += ">";
                startTag += $"${reportField.FieldName.ToUpper()}$";

                endTag += "</span>";
            }
        }
    }
}

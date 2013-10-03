using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using SheQuInfo.Models.Model;

namespace SheQuInfo.Web
{
    public static class HtmlHelper
    {
        public static MvcHtmlString CreateDisabledButtonWithMessage(this System.Web.Mvc.HtmlHelper html, string title, string msg, string cls = "btn btn-mini", string icon = "icon-trash")
        {
            return MvcHtmlString.Create(string.Format("<a href='#' class='{0}' onclick='MsgAlert(\"{1}\")'><i class='{2}'></i>{3}</a>", cls, msg, icon, title));
        }

        /// <summary>
        /// 从表格中删除数据
        /// </summary>
        /// <param name="html"></param>
        /// <param name="url"></param>
        /// <param name="targetID"></param>
        /// <returns></returns>
        public static MvcHtmlString CreateRemoveButtonInTable(this System.Web.Mvc.HtmlHelper html, string url,
         string title = "", string alt = "删除", string cls = "btn btn-mini", string icon = "icon-trash")
        {
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.AppendFormat("<a href='{0}' class='{1}' alt='{2}' onclick='DeleteItem(this);return false;'><i class='{3}'></i>{4}</a>",
                                    url, cls, alt, icon, title);
            return MvcHtmlString.Create(strBuilder.ToString());
        }

        public static MvcHtmlString DisplayColorTextWithIntValue<T>(this System.Web.Mvc.HtmlHelper<T> html,
                                                                    Expression<Func<int, bool>> expression,
                                                                    int inputValue,
                                                                    string trueColor = "green",
                                                                    string falseColor = "red", string trueString = "",
                                                                    string falseString = "")
        {
            string template = "<span style='color:{0}'>{1}</span>";
            bool isTrue = expression.Compile().Invoke(inputValue);
            string value = string.Format(template, isTrue ? trueColor : falseColor,
                                         isTrue
                                             ? string.IsNullOrEmpty(trueString) ? inputValue.ToString() : trueString
                                             : string.IsNullOrEmpty(falseString) ? inputValue.ToString() : falseString);
            return MvcHtmlString.Create(value);
        }

        /// <summary>
        /// 生成菜单列表
        /// </summary>
        /// <param name="resources">资源列表</param>
        /// <returns></returns>
        public static MvcHtmlString GetNavigates(this System.Web.Mvc.HtmlHelper html, IDictionary<string, IEnumerable<Resource>> resources)
        {
            if (resources == null || resources.Count <= 0)
                return MvcHtmlString.Empty;

            StringBuilder strBuilder = new StringBuilder();
            foreach (var item in resources)
            {
                strBuilder.AppendFormat("<li class='nav-header' title='点击收缩或展开' grouphead='{0}'><span style='float: left'>{0}</span><span style='background-image:url(/Content/img/jian.png);weight:15px;height:15px'></span><img class='imgAdd' src='/Content/img/jian.png' alt=''/></li>", item.Key);
                foreach (var rItem in item.Value)
                {
                    strBuilder.AppendFormat("<li group='{0}'><a href='{1}'>{2}</a></li>", item.Key, rItem.Url, rItem.Title);
                }
            }
            return MvcHtmlString.Create(strBuilder.ToString());
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="wvp">WebViewPage</param>
        /// <param name="imgURL">图片地址</param>
        /// <param name="alt">描述</param>
        /// <param name="eventName">事件</param>
        /// <returns></returns>
        static public MvcHtmlString ImageSubmitButton(this WebViewPage wvp, string imgURL, string id = "submit", string alt = "提交", string eventName = "document.forms[0].submit();")
        {
            string doubleQuotes = "\"{0}\"";
            Func<string, string> stringFormat = x => string.Format(doubleQuotes, x);
            string htmlString = "<a id={0} href={1}  target='_blank' onclick={2}><img src={3} alt={4} /></a>";
            return MvcHtmlString.Create(string.Format(htmlString, stringFormat(id), stringFormat("#"), stringFormat(eventName), stringFormat(imgURL), stringFormat(alt)));
        }

        /// <summary>
        /// 重定向图片连接
        /// </summary>
        /// <param name="html"></param>
        /// <param name="url"></param>
        /// <param name="hrefId"></param>
        /// <param name="title"></param>
        /// <param name="alt"></param>
        /// <param name="cls"></param>
        /// <param name="icon"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public static MvcHtmlString RedirectorWithImage(this System.Web.Mvc.HtmlHelper html, string url, string hrefId, string title, string alt, string cls, string icon, string callback)
        {
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.AppendFormat("<a id='{0}' href='{1}' class='{2}' target='_blank'><i class='{4}'></i>{5}</a>",
                string.IsNullOrEmpty(hrefId) ? "this" : hrefId,
                url, cls,
                string.IsNullOrEmpty(callback) ? "''" : callback,
                icon, title);
            return MvcHtmlString.Create(strBuilder.ToString());
        }

        public static MvcHtmlString ShowModelDialog(this System.Web.Mvc.HtmlHelper html, string url,
           string hrefId = "", string title = "", string alt = "删除",
            string cls = "btn btn-mini", string icon = "icon-trash",
            string callback = "")
        {
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.AppendFormat("<a id='{0}' href='{1}' class='{2}' onclick=\"ShowCreateOrEdit(this);return false;\"><i class='{4}'></i>{5}</a>",
                string.IsNullOrEmpty(hrefId) ? "this" : hrefId,
                url, cls,
                string.IsNullOrEmpty(callback) ? "''" : callback,
                icon, title);
            return MvcHtmlString.Create(strBuilder.ToString());
        }

        /// <summary>
        /// 文本下来列表
        /// </summary>
        /// <param name="html"></param>
        /// <param name="name"></param>
        /// <param name="select"></param>
        /// <param name="defaultText"></param>
        /// <returns></returns>
        public static MvcHtmlString TextDropdownList(this System.Web.Mvc.HtmlHelper html, string name, IEnumerable<SelectListItem> select, string defaultText = null)
        {
            StringBuilder builder = new StringBuilder("<div class='btn-group'>");
            builder.Append("<input type='text' name='" + name + "' id='" + name + "' style='float:left;' />");
            builder.Append("<button class='btn dropdown-toggle' type='button' data-toggle='dropdown'><span class='caret'></span></button>");
            builder.Append("<ul class='dropdown-menu'>");
            foreach (var item in select)
            {
                builder.AppendFormat("<li><a value='{0}' onclick='SelectIt(this)'>{1}</a></li>", item.Value ?? "", item.Text);
            }
            if (!string.IsNullOrEmpty(defaultText))
            {
                builder.AppendFormat("<li><a value='{0}' onclick='SelectIt(this)'>{1}</a></li>", "", defaultText);
            }

            builder.Append("</ul>");
            builder.Append("</div>");
            return MvcHtmlString.Create(builder.ToString());
        }

        /// <summary>
        /// 文本下来列表
        /// </summary>
        /// <param name="html"></param>
        /// <param name="name"></param>
        /// <param name="select"></param>
        /// <param name="defaultText"></param>
        /// <returns></returns>
        public static MvcHtmlString TextDropdownListFor<TModel, TValue>(this System.Web.Mvc.HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, IEnumerable<SelectListItem> select, string defaultText = null)
        {
            if (expression == null)
                throw new ArgumentNullException("expression");

            var metaData = ModelMetadata.FromLambdaExpression<TModel, TValue>(expression, html.ViewData);

            StringBuilder builder = new StringBuilder("<div class='btn-group'>");
            builder.Append(html.TextBoxFor(expression, new { style = "float:left;" }).ToHtmlString());
            builder.Append("<button class='btn dropdown-toggle' type='button' data-toggle='dropdown'><span class='caret'></span></button>");
            builder.Append("<ul class='dropdown-menu'>");
            foreach (var item in select)
            {
                builder.AppendFormat("<li><a value='{0}' onclick='SelectIt(this)'>{1}</a></li>", item.Value ?? "", item.Text);
            }
            if (!string.IsNullOrEmpty(defaultText))
            {
                builder.AppendFormat("<li><a  value='{0}' onclick='SelectIt(this)'>{1}</a></li>", "", defaultText);
            }

            builder.Append("</ul>");
            builder.Append("</div>");
            return MvcHtmlString.Create(builder.ToString());
        }
    }
}
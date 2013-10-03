using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using Wedo.Mvc.Pager;
using Wedo.Mvc.Utility;

namespace System.Web.Mvc
{
    /// <summary>
    /// MVC3 HtmlHelper 扩展2012-12-25修改
    /// </summary>
    public static class HtmlHelperExtension
    {
        #region 样式文件、脚本文件引用

        /// <summary>
        /// 引用CSS文件,默认跟目录下Css文件夹
        /// </summary>
        /// <param name="html"></param>
        /// <param name="cssfilename">CSS文件名，不含后缀</param>
        /// <returns>MvcHtmlString</returns>
        public static MvcHtmlString Css(this HtmlHelper html, params string[] cssFileName)
        {
            if (cssFileName != null)
            {
                StringBuilder strBuilder = new StringBuilder();
                foreach (string filename in cssFileName)
                {
                    if (!string.IsNullOrEmpty(filename))
                    {
                        UrlHelper URL = new UrlHelper(html.ViewContext.RequestContext);
                        string url = URL.Content("~/Content/" + filename + ".css");
                        strBuilder.AppendFormat("<link rel='Stylesheet' type='text/css' href='{0}' />", url);
                        strBuilder.AppendLine();
                        strBuilder.Append("    ");
                    }
                }

                //UrlHelper.GenerateContentUrl(folderpath + "/" + filename + ".css", html.ViewContext.HttpContext)
                return MvcHtmlString.Create(strBuilder.ToString());
            }
            return MvcHtmlString.Empty;
        }

        public static MvcHtmlString Img(this HtmlHelper html, string imgName, object htmlAttribute = null)
        {
            UrlHelper URL = new UrlHelper(html.ViewContext.RequestContext);
            string url = URL.Content("~/Content/img/" + imgName);
            IDictionary<string, object> HtmlAttributes = new Dictionary<string, object>();
            if (htmlAttribute != null)
                HtmlAttributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttribute);

            TagBuilder tag = new TagBuilder("img");
            tag.Attributes["src"] = url;
            tag.Attributes["alt"] = "Image";
            tag.MergeAttributes(HtmlAttributes);

            return MvcHtmlString.Create(tag.ToString());
        }

        /// <summary>
        /// 引用Js文件
        /// </summary>
        /// <param name="html"></param>
        /// <param name="jsFileName">Js文件名，不含后缀</param>
        /// <returns>MvcHtmlString</returns>
        public static MvcHtmlString Js(this HtmlHelper html, params string[] jsFileName)
        {
            if (jsFileName != null)
            {
                StringBuilder strBuilder = new StringBuilder();
                foreach (string filename in jsFileName)
                {
                    if (!string.IsNullOrEmpty(filename))
                    {
                        UrlHelper URL = new UrlHelper(html.ViewContext.RequestContext);
                        string url = URL.Content("~/Scripts/" + filename + ".js");
                        strBuilder.Append("<script type='text/javascript' src='" + url + "' ></script>");
                        strBuilder.AppendLine();
                        strBuilder.Append("    ");
                    }
                }
                return MvcHtmlString.Create(strBuilder.ToString());
            }
            return MvcHtmlString.Empty;
        }

        #endregion 样式文件、脚本文件引用

        #region JS

        /// <summary>
        /// Target="_blank"
        /// </summary>
        /// <param name="html"></param>
        /// <param name="text"></param>
        /// <param name="aciton"></param>
        /// <param name="routes"></param>
        /// <param name="htmlAttribute"></param>
        /// <returns></returns>
        public static MvcHtmlString BlankLink(this HtmlHelper html, string text, string aciton, object routes = null, object htmlAttribute = null)
        {
            IDictionary<string, object> HtmlAttributes = new Dictionary<string, object>();
            if (htmlAttribute != null)
                HtmlAttributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttribute);
            HtmlAttributes["target"] = "_blank";

            RouteValueDictionary rv = new RouteValueDictionary();
            if (routes != null)
                rv = new RouteValueDictionary(routes);

            return html.ActionLink(text, aciton, rv, HtmlAttributes);
        }

        /// <summary>
        /// 创建弹窗链接
        /// </summary>
        /// <param name="html"></param>
        /// <param name="controller"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static MvcHtmlString CreateLink(this  HtmlHelper html, string controller = null, string action = "Create", string text = "新建")
        {
            UrlHelper URL = new UrlHelper(html.ViewContext.RequestContext);
            string url = "";
            if (controller.IsNullOrWhiteSpace())
                url = URL.Action(action);
            else
                url = URL.Action(action, controller);
            string astr = "<a href='{0}' target='_blank' class='btn btn-primary' onclick='ShowCreateOrEdit(this);return false;'><i class='icon-edit'></i>{1}</a>";
            return MvcHtmlString.Create(string.Format(astr, url, text));
        }

        /// <summary>
        /// 详细信息
        /// </summary>
        /// <param name="html"></param>
        /// <param name="id"></param>
        /// <param name="controller"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static MvcHtmlString DetailsLink(this  HtmlHelper html, object id, string text = "查看", string controller = null, string action = "Details")
        {
            RouteValueDictionary dict = new RouteValueDictionary();

            dict.Add("id", id);
            if (controller.IsNotNullOrWhiteSpace())
                dict.Add("Controller", controller);

            Dictionary<string, object> hAttrs = new Dictionary<string, object>();
            hAttrs.Add("onclick", "ShowDetails(this);return false;");
            hAttrs.Add("target", "_blank");

            return html.ActionLink(text, action, dict, hAttrs);
        }

        /// <summary>
        /// 修改弹窗链接
        /// </summary>
        /// <param name="html"></param>
        /// <param name="id"></param>
        /// <param name="controller"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static MvcHtmlString EditLink(this  HtmlHelper html, object id, string text = "修改", string controller = null, string action = "Edit")
        {
            UrlHelper URL = new UrlHelper(html.ViewContext.RequestContext);
            string url = "";
            if (controller.IsNullOrWhiteSpace())
                url = URL.Action(action, new { id });
            else
                url = URL.Action(action, new { id, controller = controller });
            string astr = "<a href='{0}' target='_blank' class='btn btn-mini' onclick='ShowCreateOrEdit(this);return false;'><i class='icon-edit'></i>{1}</a>";
            return MvcHtmlString.Create(string.Format(astr, url, text));
        }

        /// <summary>
        /// 给链接添加JS操作
        /// </summary>
        /// <param name="html"></param>
        /// <param name="text"></param>
        /// <param name="action"></param>
        /// <param name="routeValues"></param>
        /// <param name="js"></param>
        /// <returns></returns>
        public static MvcHtmlString JSLink(this HtmlHelper html, string text, string action, string js, object routeValues = null, object htmlAttribute = null)
        {
            RouteValueDictionary dict = new RouteValueDictionary();
            if (routeValues != null)
                dict = new RouteValueDictionary(routeValues);

            Dictionary<string, object> hAttrs = new Dictionary<string, object>();
            if (htmlAttribute != null)
                htmlAttribute = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttribute);

            hAttrs.Add("onclick", js + ";return false;");
            hAttrs.Add("target", "_blank");
            hAttrs.Add("class", "btn");

            return html.ActionLink(text, action, dict, hAttrs);
        }

        public static MvcHtmlString ShortcutLink(this  HtmlHelper html, string text, string action, object routeVlaues = null, string call = null)
        {
            UrlHelper URL = new UrlHelper(html.ViewContext.RequestContext);
            string url = "";
            if (routeVlaues != null)
            {
                url = URL.Action(action, new RouteValueDictionary(routeVlaues));
            }
            else
            {
                url = URL.Action(action);
            }
            string click = "";
            if (call.IsNullOrWhiteSpace())
            {
                click = "ShortcutLink(this);return false;";
            }
            else
            {
                click = "ShortcutLink(this," + call + ");return false;";
            }

            string astr = "<a href='{0}' target='_blank' class='btn btn-mini' onclick='{1}'><i class='icon-edit'></i>{2}</a>";
            return MvcHtmlString.Create(string.Format(astr, url, click, text));
        }

        public static MvcHtmlString ShowEditDialog(this  HtmlHelper html, string text, string action, object routeVlaues = null, string btnclass = "btn-mini")
        {
            UrlHelper URL = new UrlHelper(html.ViewContext.RequestContext);
            string url = "";
            if (routeVlaues != null)
            {
                url = URL.Action(action, new RouteValueDictionary(routeVlaues));
            }
            else
            {
                url = URL.Action(action);
            }

            string astr = "<a href='{0}' target='_blank' class='btn {1}' onclick='ShowCreateOrEdit(this);return false;'><i class='icon-edit'></i>{2}</a>";
            return MvcHtmlString.Create(string.Format(astr, url, btnclass, text));
        }

        #endregion JS

        #region Select

        public static MvcHtmlString CheckBoxList(this HtmlHelper html, string InputName, IEnumerable<SelectListItem> list, object htmlAttributes = null)
        {
            IDictionary<string, object> HtmlAttributes = new Dictionary<string, object>();
            if (htmlAttributes != null)
                HtmlAttributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);

            StringBuilder strBuilder = new StringBuilder();
            if (list != null)
            {
                foreach (var item in list)
                {
                    HtmlAttributes["value"] = item.Value;
                    TagBuilder tag = new TagBuilder("input");
                    tag.MergeAttributes(HtmlAttributes, true);
                    tag.Attributes.Add("Name", InputName);
                    tag.Attributes.Add("type", "checkbox");
                    if (item.Selected)
                    {
                        tag.Attributes.Add("checked", "checked");
                    }
                    strBuilder.Append("<label>" + tag.ToString() + item.Text + "</label>");
                }
            }
            return new MvcHtmlString(strBuilder.ToString());
        }

        /// <summary>
        /// 可空布尔DropdownList
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="?"></typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="expression"></param>
        /// <param name="optionLabel"></param>
        /// <returns></returns>
        public static MvcHtmlString DropDownListForBool<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression, string optionLabel = "请选择")
        {
            List<SelectListItem> list = new List<SelectListItem>();
            if (typeof(TProperty) == typeof(Nullable<bool>))
            {
                list.Add(new SelectListItem { Text = optionLabel, Value = "" });
                list.Add(new SelectListItem { Text = "是", Value = bool.TrueString });
                list.Add(new SelectListItem { Text = "否", Value = bool.FalseString });
            }
            else if (typeof(TProperty) == typeof(bool))
            {
                list.Add(new SelectListItem { Text = "是", Value = bool.TrueString });
                list.Add(new SelectListItem { Text = "否", Value = bool.FalseString });
            }
            return htmlHelper.DropDownListFor(expression, list);
        }

        public static MvcHtmlString RadioList(this HtmlHelper html, string InputName, IEnumerable<SelectListItem> list, object htmlAttributes = null)
        {
            IDictionary<string, object> HtmlAttributes = new Dictionary<string, object>();
            if (htmlAttributes != null)
                HtmlAttributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            StringBuilder strBuilder = new StringBuilder();
            if (list != null)
            {
                foreach (var item in list)
                {
                    strBuilder.Append("<label>" + html.RadioButton(InputName, item.Value, item.Selected, HtmlAttributes) + item.Text + "</label>");
                }
            }
            return new MvcHtmlString(strBuilder.ToString());
        }

        public static MvcHtmlString RadioListForBool(this HtmlHelper html, string IputName, object htmlAttributes = null)
        {
            List<SelectListItem> list = new List<SelectListItem> {
                new SelectListItem{ Text="是", Value="True"},
                new SelectListItem{ Text="否", Value="False"}
            };
            return RadioList(html, IputName, list, htmlAttributes);
        }

        #endregion Select

        #region Input

        /// <summary>
        /// 显示时间控件
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="expression"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static MvcHtmlString DisplayDatetimeFor<TModel>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, System.DateTime?>> expression, string format = "yyyy-MM-dd")
        {
            System.DateTime? val = expression.Compile()(htmlHelper.ViewData.Model);
            if (val == null || val == DateTime.MinValue)
                return MvcHtmlString.Empty;
            else
                return MvcHtmlString.Create(val.Value.ToString(format));
        }

        /// <summary>
        /// 显示时间控件
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="expression"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static MvcHtmlString DisplayDatetimeFor<TModel>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, System.DateTime>> expression, string format = "yyyy-MM-dd")
        {
            System.DateTime val = expression.Compile()(htmlHelper.ViewData.Model);
            if (val == null || val == DateTime.MinValue)
                return MvcHtmlString.Empty;
            else
                return MvcHtmlString.Create(val.ToString(format));
        }

        /// <summary>
        /// 获取属性的DisplayName
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="expression"></param>
        /// <param name="requiredValid"></param>
        /// <returns></returns>
        public static MvcHtmlString DisplayNameFor<TModel, TProperty>(
           this HtmlHelper<TModel> htmlHelper,
           Expression<Func<TModel, TProperty>> expression, bool? requiredValid = null, bool iscolon = false
       )
        {
            var metaData = ModelMetadata.FromLambdaExpression<TModel, TProperty>(expression, htmlHelper.ViewData);
            string value = metaData.DisplayName ?? (metaData.PropertyName ?? ExpressionHelper.GetExpressionText(expression));
            if (iscolon)
                value += "：";
            if ((metaData.IsRequired && requiredValid == null) || requiredValid == true)
            {
                value += "<span class='required'>*</span>";
            }

            return MvcHtmlString.Create(value);
        }

        /// <summary>
        /// 编辑时间控件
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static MvcHtmlString EditDateTimeFor<TModel>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, System.DateTime?>> expression, string format = "yyyy-MM-dd")
        {
            System.DateTime? val = expression.Compile()(htmlHelper.ViewData.Model);
            if (val == null || val == DateTime.MinValue)
                return htmlHelper.TextBoxFor(expression, new { @class = "datetime", @value = "" });
            else
                return htmlHelper.TextBoxFor(expression, new { @class = "datetime", @value = val.Value.ToString(format) });
        }

        /// <summary>
        /// 编辑时间控件
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static MvcHtmlString EditDateTimeFor<TModel>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, System.DateTime>> expression, string format = "yyyy-MM-dd")
        {
            System.DateTime val = expression.Compile()(htmlHelper.ViewData.Model);
            if (val == null || val == DateTime.MinValue)
                return htmlHelper.TextBoxFor(expression, new { @class = "datetime", @value = "" });
            else
                return htmlHelper.TextBoxFor(expression, new { @class = "datetime", @value = val.ToString(format) });
        }

        /// <summary>
        /// 关键字搜索
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="desc"></param>
        /// <param name="buttons"></param>
        /// <returns></returns>
        public static MvcHtmlString KeywordSearch(this HtmlHelper htmlHelper, string desc = "请输入关键字：",
            string searchJs = "Search(this)", bool isRefresh = true)
        {
            // StringBuilder builder = new StringBuilder("<div class='input-prepend input-append' style='display: inline;margin-left:5px'>");
            StringBuilder builder = new StringBuilder("<div style='display: inline;margin-left:5px'>");
            builder.Append("<span class='add-on'>" + desc + "</span><input name='Keyword' id='Keyword' type='text' class='span2'  onfocus='keyWordEnter()' />");

            builder.Append("<button type='button' onclick='" + searchJs + "' id='btnSearchCondition' class='btn btn-primary'>搜索</button>");
            if (isRefresh)
                builder.Append("<button type='button' onclick='RefreshSearchContent()' id='btnRefresh' class='btn'>刷新</button>");
            builder.Append("</div>");
            return MvcHtmlString.Create(builder.ToString());
        }

        /// <summary>
        /// 生成在Label中的TextBox
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="expression"></param>
        /// <param name="requiredValid"></param>
        /// <returns></returns>
        public static MvcHtmlString LebelTextBoxFor<TModel, TProperty>(
           this HtmlHelper<TModel> htmlHelper,
           Expression<Func<TModel, TProperty>> expression, bool? requiredValid = null, bool iscolon = true
       )
        {
            StringBuilder builder = new StringBuilder("<label>");
            builder.Append(htmlHelper.DisplayNameFor(expression, requiredValid, iscolon))
                .Append(htmlHelper.TextBoxFor(expression))
                .Append("</label>");
            return MvcHtmlString.Create(builder.ToString());
        }

        public static MvcHtmlString TextBoxBootStrap<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, object htmlAttr = null)
        {
            IDictionary<string, object> HtmlAttributes = new Dictionary<string, object>();
            if (htmlAttr != null)
                HtmlAttributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttr);
            HtmlAttributes.Add("class", "form-control");
            return htmlHelper.TextBoxFor(expression, HtmlAttributes);
        }

        /// <summary>
        /// 只读
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="expression"></param>
        /// <param name="isReadOnly"></param>
        /// <returns></returns>
        public static MvcHtmlString TextBoxReadOnlyFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, object htmlAttr = null)
        {
            IDictionary<string, object> HtmlAttributes = new Dictionary<string, object>();
            if (htmlAttr != null)
                HtmlAttributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttr);
            HtmlAttributes.Add("ReadOnly", "ReadOnly");

            return htmlHelper.TextBoxFor(expression, HtmlAttributes);
        }

        /// <summary>
        /// 生成带有查询按钮的文本框
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="expression">属性</param>
        /// <param name="clickFunction">点击调用的方法</param>
        /// <param name="htmlAttributes">文本属性</param>
        /// <returns></returns>
        public static MvcHtmlString TextSearch<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, string clickFunction, object htmlAttributes = null, bool isEnter = true)
        {
            IDictionary<string, object> HtmlAttributes = new Dictionary<string, object>();
            if (htmlAttributes != null)
                HtmlAttributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);

            string id = StringHelper.RandomString(5);
            HtmlAttributes.Add("TextSearchID", id);
            HtmlAttributes.Add("class", "span2");
            HtmlAttributes.Add("onfocus", "keyWordEnter()");

            StringBuilder builder = new StringBuilder();
            builder.Append("<div class='input-append'>");
            builder.Append(htmlHelper.TextBoxFor(expression, HtmlAttributes).ToHtmlString());
            builder.Append("<button class='btn' type='button' style='padding:4px' onclick='" + clickFunction + "'><i class='icon-search'></i></button></div>");

            if (isEnter)
            {
                string selector = "input[TextSearchID=" + id + "]";
                builder.Append("<script type='text/javascript'>$('" + selector + "').keydown(function (e) {if (e.keyCode == 13) {e.preventDefault(); " + clickFunction + ";}}); </script>");
            }

            return MvcHtmlString.Create(builder.ToString());
        }

        /// <summary>
        /// 生成带有查询按钮的文本框
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="name">文本名</param>
        /// <param name="clickFunction">点击调用的方法</param>
        /// <param name="htmlAttributes">文本属性</param>
        /// <returns></returns>
        public static MvcHtmlString TextSearch(this HtmlHelper htmlHelper, string name, string clickFunction, object htmlAttributes = null, bool isEnter = true)
        {
            IDictionary<string, object> HtmlAttributes = new Dictionary<string, object>();
            if (htmlAttributes != null)
                HtmlAttributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);

            string id = Guid.NewGuid().ToString();
            HtmlAttributes.Add("TextSearchID", id);

            StringBuilder builder = new StringBuilder();
            builder.Append("<div class='input-append'>");
            builder.Append(htmlHelper.TextBox(name, "", HtmlAttributes).ToHtmlString());
            builder.Append("<button class='btn' type='button' style='padding:4px' onclick='" + clickFunction + "'><i class='icon-search'></i></button></div>");
            if (isEnter)
            {
                string selector = "input[TextSearchID=" + id + "]";

                builder.Append("<script type='text/javascript'>$('" + selector + "').keydown(function (e) {if (e.keyCode == 13) {e.preventDefault(); " + clickFunction + ";}}); </script>");
            }
            return MvcHtmlString.Create(builder.ToString());
        }

        #endregion Input

        #region 分页

        /// <summary>
        /// 扩展方法，将常用的PagerOptions 封装了一下.
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="pagedList"></param>
        /// <param name="model"></param>
        /// <param name="htmlAttributes"></param>
        /// <returns></returns>
        public static MvcHtmlString CommonPager(this HtmlHelper helper, IPagedList pagedList, object searchModel = null, string action = "Search", object htmlAttributes = null)
        {
            PagerOptions pagerOptions = new PagerOptions()
            {
                PageIndexParameterName = "page",
                ShowDisabledPagerItems = false,
                AlwaysShowFirstLastPageNumber = true,

                ContainerTagName = "div",
                CssClass = "pagedlist",
                CurrentPagerItemWrapperFormatString = " <a href='{url}' class='active'>{0}</a>"
            };
            if (searchModel != null)
            {
                RouteValueDictionary rvd = new RouteValueDictionary(searchModel);

                if (!string.IsNullOrEmpty(action))
                {
                    rvd["action"] = action;
                }

                return helper.Pager(pagedList, pagerOptions, "Default", rvd, new RouteValueDictionary(htmlAttributes));
            }
            else
            {
                var obj = new { Action = action };
                return helper.Pager(pagedList, pagerOptions, "Default", obj, htmlAttributes);
            }
        }

        #endregion 分页

        #region ValidationExtensions

        public static MvcHtmlString ValidationSummaryBootStrap(this HtmlHelper helper, bool excludePropertyErrors, string message)
        {
            StringBuilder sb = new StringBuilder("<div class='alert alert-block alert-danger fade in'><button type='button' class='close' data-dismiss='alert' aria-hidden='true'>×</button>");

            if (helper.ViewData.ModelState.IsValid)
                return MvcHtmlString.Empty;
            if (!string.IsNullOrEmpty(message))
            {
                sb.AppendFormat("<h4>{0}</h4>", message);
            }
            if (excludePropertyErrors)
            {
                if (helper.ViewData.ModelState.Keys.Count > 0)
                {
                    sb.Append("<ul>");
                    foreach (var key in helper.ViewData.ModelState.Keys)
                    {
                        foreach (var err in helper.ViewData.ModelState[key].Errors)
                            sb.AppendFormat("<li>{0}</li>", helper.Encode(err.ErrorMessage));
                    }
                    sb.Append("</ul>");
                }
            }
            sb.Append("</div>");
            return MvcHtmlString.Create(sb.ToString());
        }

        #endregion ValidationExtensions

        #region 其他

        #endregion 其他
    }
}
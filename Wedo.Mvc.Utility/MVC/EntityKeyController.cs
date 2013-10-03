using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Wedo.Mvc.Pager;
using System.ComponentModel;

namespace Wedo.Mvc.Utility
{
    public abstract class EntityKeyController<T,TKey,TSearch> : BaseController
     where T : class where TSearch:class
    {
        public EntityKeyController()
        {
            ViewBag.EntityName = EntityName;
        }

        #region 公共属性
        protected readonly string UnKnownErrorMSG = "错误，请联系管理员！<br/>错误详情： {0}";
        protected static int PageSize = 10;
        /// <summary>
        /// 本控制器操作实体的中文名称
        /// </summary>
        public virtual string EntityName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_EntityName))
                {
                    var type = typeof(T);
                    var desc = type.GetCustomAttributes(false)
                        .OfType<DisplayNameAttribute>().FirstOrDefault();
                    if (desc != null)
                        _EntityName = desc.DisplayName;
                    else
                        _EntityName = type.Name;
                }
                return _EntityName;
            }
        }
        private static string _EntityName;

        #endregion

        #region 公用方法

        #region 数据操作

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="model"></param>
        protected abstract void Insert(T model);

        /// <summary>
        /// 根据ID删除数据
        /// </summary>
        /// <param name="id"></param>
        protected abstract void DeleteModel(TKey id);

        /// <summary>
        /// 修改对象
        /// </summary>
        /// <param name="model"></param>
        protected abstract void UpdateModel(T model, TKey id);

        /// <summary>
        /// 查询单个数据
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        protected abstract T FindById(TKey Id);

        /// <summary>
        /// 根据条件查询分页数据
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        protected abstract PagedList<T> GetSearchPagedList(TSearch where);
 
        #endregion

        #region Action

        /// <summary>
        /// 主页
        /// </summary>
        /// <returns></returns>
        public virtual ActionResult Index()
        {
            return Search(null);
        }

        /// <summary>
        /// 搜索方法
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public virtual ActionResult Search(TSearch where)
        {
            ViewData["SearchModel"] = where;

            var list = GetSearchPagedList(where);
            return ReturnAjaxView(list);
        }


        /// <summary>
        /// 根据ID得到对象的Json类型的 数据
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public JsonResult GetById(TKey Id)
        {
            T entity = FindById(Id);
            return Json(entity);
        }

        public virtual ActionResult Details(TKey id)
        {
            return ReturnAjaxView(FindById(id));
        }

        public virtual ActionResult Create()
        {
            return ReturnAjaxView();
        }

        [HttpPost]
        public virtual ActionResult Create(T model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //修改数据
                    Insert(model);
                    return SuccessMsg("修改成功！");
                }
                else
                {
                    var list = ModelState.Values.Where(m => m.Errors.Count > 0)
                        .Select(m => m.Errors.Select(s => s.ErrorMessage));
                    return ErrorMSG("验证失败：" + string.Join(".", list));

                }
            }
            catch (Exception ex)
            {
                return SuccessMsg(ex.Message);
            }
        }

        public virtual ActionResult Edit(TKey id)
        {
            return ReturnAjaxView(FindById(id));

        }

        [HttpPost]
        public virtual ActionResult Edit(T model, TKey id)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //修改数据
                    UpdateModel(model, id);
                    return SuccessMsg("修改成功！");
                }
                else
                {
                    var list = ModelState.Values.Where(m => m.Errors.Count > 0)
                        .Select(m => m.Errors.Select(s=>s.ErrorMessage));
                    return ErrorMSG("验证失败："+string.Join(".", list));

                }
            }
            catch (Exception ex)
            {
                return SuccessMsg(ex.Message);
            }
         }

        [HttpPost]
        public virtual ActionResult Delete(TKey id)
        {
            try
            {
                DeleteModel(id); 
                return SuccessMsg("删除成功！");
            }
            catch (Exception ex)
            {
               return SuccessMsg( ex.Message);
            }
         }
        #endregion

        #endregion


    }
}

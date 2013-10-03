using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web;
using System.Linq.Expressions;
using System.Globalization;
using Wedo.Mvc.Pager; 


namespace Wedo.Mvc.Utility
{
    public abstract class CommonController<T> : BaseController
     where T : class
    {
        public CommonController()
        {
            ViewBag.EntityName = EntityName;
        }
        #region 公共属性
        protected readonly string UnKnownErrorMSG = "错误，请联系管理员！<br/>错误详情： {0}";


        /// <summary>
        /// 本控制器操作实体的中文名称
        /// </summary>
        public abstract string EntityName { get; }

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
        protected abstract void DeleteModel(object id);

        /// <summary>
        /// 修改对象
        /// </summary>
        /// <param name="model"></param>
        protected abstract void UpdateModel(T model, object id);

        /// <summary>
        /// 查询单个数据
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        protected abstract T FindById(object Id);

        /// <summary>
        /// 根据条件查询分页数据
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        protected abstract PagedList<T> GetSearchPagedList(SearchModel where);


        protected virtual ResultMSG Add(T model)
        {
            ResultMSG rep = new ResultMSG();
            try
            {
                Insert(model);

                rep.Result = true;
                rep.MSG = "Success!";
            }
            catch (Exception ex)
            {
                rep.MSG = string.Format(UnKnownErrorMSG, ex.Message);
            }
            return rep;
        }

        protected virtual ResultMSG Remove(object Id)
        {
            ResultMSG rep = new ResultMSG();
            try
            {
                //数据删除
                DeleteModel(Id);

                rep.Result = true;
                rep.MSG = string.Format("恭喜你，你已成功删除了Id为{0}的{1}！", Id.ToString(), EntityName);
            }
            catch (Exception ex)
            {
                rep.Result = false;
                rep.MSG = string.Format(UnKnownErrorMSG, ex.Message);
            }
            return rep;
        }

        protected virtual ResultMSG Update(T model, object id)
        {
            ResultMSG rep = new ResultMSG();
            try
            {

                //修改数据
                UpdateModel(model, id);

                rep.Result = true;
                rep.MSG = string.Format("成功，你成功的修改了 {0},编号为{1}！", EntityName,id);
            }
            catch (Exception ex)
            {
                rep.MSG = string.Format(UnKnownErrorMSG, ex.Message);
            }
            return rep;
        }

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
        public virtual ActionResult Search(SearchModel where)
        {
            if (where == null)
            {
                where = new SearchModel();
            }
            else
            {
                where.Keyword = (where.Keyword ?? "").Trim();
            }

            
            ViewData["SearchModel"] = where;

            var list = GetSearchPagedList(where);
            return ReturnAjaxView(list);
        }


        /// <summary>
        /// 根据ID得到对象的Json类型的 数据
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public JsonResult GetById(object Id)
        {
            T entity = FindById(Id);
            return Json(entity);
        }

        public virtual ActionResult Details(object id)
        {
            if (Request.IsAjaxRequest())
                return PartialView(FindById(id));
            else
                return View(FindById(id));
        }

        public virtual ActionResult Create()
        {
            return ReturnAjaxView ();
        }

        [HttpPost]
        public virtual ActionResult Create(T model)
        {
            return Json(Add(model));
        }

        public virtual ActionResult Edit(object id)
        {

            return ReturnAjaxView(FindById(id));

        }

        [HttpPost]
        public virtual ActionResult Edit(T model, object id)
        {
            return Json(Update(model, id));
        }

        [HttpPost]
        public virtual ActionResult Delete(object id)
        {
            var result = Remove(id);

            if (Request.IsAjaxRequest())
                return Json(result);
            else
                return RedirectToAction("Index");
        }
        #endregion

        #endregion
    }

}

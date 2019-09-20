
 
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using JiraPortal.Controllers.Models;
using JiraPortal.Controllers.Data;
using JiraPortal.Controllers.Services;
Namespace JiraPortal.Controllers
ModelTypeName DataCallMetric
ModelVariable dataCallMetric
ControllerRootName DataCallMetrics
ControllerName DataCallMetricsController 


namespace JiraPortal.Controllers.Controllers
{
    public class DataCallMetricsController : Controller
    {
        private readonly IDataCallMetricService _dataCallMetricService; 
        public DataCallMetricsController(
            IDataCallMetricService DataCallMetricService )
        {
            this._dataCallMetricService = DataCallMetricService; 
        }
        public DataCallMetricsController()
        {

        }
        [NonAction]
        protected virtual DataCallMetricViewModel PrepareViewModel(DataCallMetric dataCallMetric )
        {
            if (dataCallMetric == null)
                throw new ArgumentNullException("DataCallMetric");
                 
            var model = new DataCallMetricViewModel
            {
                DataCallMetric = dataCallMetric
            };
            return model;
        } 
        public ActionResult Index()
        {
            var items = new List<DataCallMetricViewModel>();
            IEnumerable<DataCallMetric> dataCallMetrics = _dataCallMetricService.GetAll(); 

            foreach (var item in dataCallMetrics) 
                items.Add(PrepareViewModel(item));
    
            return View(items.ToList());
        } 
        public ActionResult Details(int id)
        { 
            DataCallMetric dataCallMetric = _dataCallMetricService.GetById(id);
            if (dataCallMetric == null)
            {
                return HttpNotFound();
            }
            return View(dataCallMetric);
        } 
        public ActionResult Create()
        {
            var vm = new DataCallMetricViewModel();

            vm.DataCallMetric = new DataCallMetric()
            {
                DataCallMetricId = 0 
            }; 
            return View(vm); 
        } 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(DataCallMetricViewModel VM)
        {
            DataCallMetric dataCallMetric = VM.DataCallMetric;
            if (ModelState.IsValid)
            { 
                _dataCallMetricService.Insert(dataCallMetric);
                return RedirectToAction("Index");
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors); 
            }
            return View(dataCallMetric);
        } 
        public ActionResult Edit(int id)
        {
            var dataCallMetric = _dataCallMetricService.GetById(id);
            DataCallMetricViewModel vm = PrepareViewModel( dataCallMetric );
              
            return View(vm);
        } 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(DataCallMetricViewModel vm)
        {
            if (ModelState.IsValid)
            {
                 _dataCallMetricService.Update(vm.DataCallMetric);
                return RedirectToAction("Index");
            }
            return View(vm);
        } 
        public ActionResult Delete(int id)
        {
            DataCallMetric dataCallMetric = _dataCallMetricService.GetById(id);
            if (dataCallMetric == null)
                return HttpNotFound();
                 
            return View(dataCallMetric);
        } 
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DataCallMetric dataCallMetric = _dataCallMetricService.GetById(id);
            _dataCallMetricService.Delete(dataCallMetric);
            return RedirectToAction("Index");
        } 
    }
 
    public interface IDataCallMetricService
    {
        void Delete(DataCallMetric dataCallMetric);
        IQueryable<DataCallMetric> GetAll();
        DataCallMetric GetById(int DataCallMetricId);
        void Insert(DataCallMetric dataCallMetric);
        void Update(DataCallMetric dataCallMetric);
    }
    public class DataCallMetricService : IDataCallMetricService
    {
        private readonly IRepository<DataCallMetric> _dataCallMetricRepository; 
        public DataCallMetricService()
        {
        }
        public DataCallMetricService(IRepository<DataCallMetric> dataCallMetricRepository)
        {
            _DataCallMetricRepository = dataCallMetricRepository;
        }
        public virtual DataCallMetric GetById(int DataCallMetricId)
        {
            DataCallMetric dataCallMetric = _dataCallMetricRepository.GetById(DataCallMetricId);
            return dataCallMetric;
        }
        public virtual IQueryable<DataCallMetric> GetAll()
        {
            IQueryable<DataCallMetric> dataCallMetrics = _dataCallMetricRepository.Table;
            return dataCallMetrics;
        }
        public virtual void Insert(DataCallMetric dataCallMetric)
        {
            if (dataCallMetric == null)
                throw new ArgumentNullException("DataCallMetric");
            _dataCallMetricRepository.Insert(dataCallMetric);
        }
        public virtual void Update(DataCallMetric dataCallMetric)
        {
            if (dataCallMetric == null)
                throw new ArgumentNullException("DataCallMetric");
            _dataCallMetricRepository.Update(dataCallMetric);
        }
        public virtual void Delete(DataCallMetric dataCallMetric)
        {
            if (dataCallMetric == null)
                throw new ArgumentNullException("DataCallMetric");
            _DataCallMetricRepository.Delete(dataCallMetric);
        }
    }
 }    
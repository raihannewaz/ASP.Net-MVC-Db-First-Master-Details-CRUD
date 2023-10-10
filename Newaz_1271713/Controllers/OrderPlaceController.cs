using Newaz_1271713.Models.ViewModels;
using Newaz_1271713.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using System.Security.Cryptography;
using System.Data.Entity;


namespace Newaz_1271713.Controllers
{
    public class OrderPlaceController : Controller
    {

        private myDBEntities db = new myDBEntities();
        //List<Customer> cust = new List<Customer>();
        List<Order> or = new List<Order>();


        public ActionResult Create(int? customerID = null)
        {
            CustomerOrderVM vm = null;
            if (customerID != null)
            {
                or = (from d in db.Orders where d.CustomerID == customerID select d).ToList();
                Customer c = db.Customers.Find(customerID);
                vm = new CustomerOrderVM { CustomerID = c.CustomerID };
            }

            var item = db.Items.ToList();
            var itemVM = item.Select(i => new CustomerOrderVM
            {
                ItemID = i.ItemID,
                ItemName = i.ItemName
            }).ToList();
            ViewBag.ItemList = new SelectList(itemVM, "ItemID", "ItemName");



            TempData["or"] = or;

            return View(vm);
        }

        [HttpPost]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult create(CustomerOrderVM vm, string ButtonType)
        {
            if (ButtonType == "Add")
            {
                AddM(vm);
                return PartialView("_PartialPage1");
            }
            else if (ButtonType == "Save")
                SaveM(vm);
            else
                DeleteM(vm.CustomerID);
            return Json(new { url = Url.Action("list") });
        }


        [Route("OrderDetails")]
        public ActionResult List()
        {
            List<CustomerOrderVM> coVM = new List<CustomerOrderVM>();

            var customerOrders = (from c in db.Customers
                                  join o in db.Orders on c.CustomerID equals o.CustomerID
                                  select new { c, o }).ToList();

            foreach (var i in customerOrders)
            {
                var co = new CustomerOrderVM
                {
                    CustomerID = i.c.CustomerID,
                    CustomerName = i.c.CustomerName,
                    CustomerAddress = i.c.CustomerAddress,
                    ImagePath = i.c.ImagePath,
                    OrderNo = i.o.OrderNo,
                    OrderDate = (DateTime)i.o.OrderDate,
                    OrderStatus = i.o.OrderStatus
                };
                coVM.Add(co);
            }

            return View(coVM);
        }


        public void AddM(CustomerOrderVM vm)
        {
            or = TempData["or"] as List<Order>;
            if (or == null)
                or = new List<Order>();
            or.Add(new Order() { OrderID = vm.OrderID, OrderNo = vm.OrderNo, ItemID = vm.ItemID, OrderDate = vm.OrderDate, OrderStatus = vm.OrderStatus = true });

            ViewBag.records = or;
            TempData["or"] = or;
        }

        public void SaveM(CustomerOrderVM vm)
        {
            DeleteM(vm.CustomerID);
            Customer c = new Customer() { CustomerID = vm.CustomerID, CustomerName = vm.CustomerName, CustomerAddress = vm.CustomerAddress, ImagePath = vm.ImagePath };
            db.Customers.Add(c);
            db.SaveChanges();
            or = TempData["or"] as List<Order>;
            foreach (Order d in or)
            {
                Order o = new Order() { OrderID = vm.OrderID, OrderNo = vm.OrderNo, ItemID = vm.ItemID, CustomerID = vm.CustomerID, OrderDate = vm.OrderDate, OrderStatus = vm.OrderStatus };
                db.Orders.Add(o);
                db.SaveChanges();
            }

            TempData["or"] = "";
            Session["or"] = "";
        }

        public void DeleteM(int CustomerID)
        {
            db.Database.ExecuteSqlCommand($"delete Orders where CustomerID='{CustomerID}'");
            db.Database.ExecuteSqlCommand($"delete Customer where CustomerId='{CustomerID}'");
            db.SaveChanges();
        }

        public ActionResult Delete(int id)
        {
            var customer = db.Customers.Include(c => c.Orders).FirstOrDefault(c => c.CustomerID == id);

            if (customer == null)
            {
                return HttpNotFound();
            }

            db.Orders.RemoveRange(customer.Orders);
            db.Customers.Remove(customer);
            db.SaveChanges();

            return RedirectToAction("List");
        }



        public ActionResult Edit(int id)
        {
            var customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }

            var orders = db.Orders.Where(o => o.CustomerID == id).ToList();

            var viewModel = new CustomerOrderVM
            {
                Customers = customer,
                Orders = orders
            };

            return View(viewModel);
        }

        [HttpPost]
  
        public ActionResult Edit(CustomerOrderVM vm)
        {
            if (ModelState.IsValid)
            {
                db.Entry(vm.Customers).State = EntityState.Modified;

                foreach (var order in vm.Orders)
                {
                    db.Entry(order).State = EntityState.Modified;
                }

                db.SaveChanges();

                return RedirectToAction("List");
            }

            return View(vm);
        }

    }
}
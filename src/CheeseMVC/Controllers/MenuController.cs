using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CheeseMVC.Data;
using CheeseMVC.ViewModels;
using CheeseMVC.Models;
using Microsoft.EntityFrameworkCore;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CheeseMVC.Controllers
{
    public class MenuController : Controller
    {
        private readonly CheeseDbContext context;

        public MenuController(CheeseDbContext dbContext)
        {
            context = dbContext;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            IEnumerable<Menu> cheeseMenus = context.Menu.ToList();
            return View(cheeseMenus);
        }

        public IActionResult Add()
        {
            AddMenuViewModel addMenuViewModel = new AddMenuViewModel();
            return View(addMenuViewModel);
        }

        [HttpPost]
        public IActionResult Add(AddMenuViewModel addMenuViewModel)
        {
            if (ModelState.IsValid)
            {
                Menu newMenu = new Menu()
                {
                    Name = addMenuViewModel.Name
                };

                context.Menu.Add(newMenu);
                context.SaveChanges();
                return Redirect("/Menu/ViewMenu/" + newMenu.ID);

            }

            return View(addMenuViewModel);
        }

        
        public IActionResult ViewMenu(int id)
        {
            var menu = context.Menu.Single(m => m.ID == id);

            List<CheeseMenu> items = context
                .CheeseMenu
                .Include(item => item.Cheese)
                .Where(cm => cm.MenuID == id)
                .ToList();

            ViewMenuViewModel viewMenuViewModel = new ViewMenuViewModel
            {
                Menu = menu,
                Items = items
            };

            return View(viewMenuViewModel);


        }

        public IActionResult AddItem(int id)
        {
            //return Content(id.ToString());
            Menu menu = context.Menu.Single(m => m.ID == id);
            //return Content(menu.Name);

            List<Cheese> cheeses = context.Cheeses.ToList();            
            return View(new AddMenuItemViewModel(menu, cheeses));

        }

        [HttpPost]
        public IActionResult AddItem(AddMenuItemViewModel addMenuItemViewModel)
        {
            if (ModelState.IsValid)
            {
                var cheeseID = addMenuItemViewModel.CheeseID;
                var menuID = addMenuItemViewModel.MenuID;

                IList<CheeseMenu> existingItems = context.CheeseMenu
                    .Where(cm => cm.CheeseID == cheeseID)
                    .Where(cm => cm.MenuID == menuID).ToList();

                if (existingItems.Count == 0)
                {
                    CheeseMenu menuItem = new CheeseMenu
                    {
                        Cheese = context.Cheeses.Single(c => c.ID == cheeseID),
                        Menu = context.Menu.Single(m => m.ID == menuID)
                    };

                    context.CheeseMenu.Add(menuItem);
                    context.SaveChanges();
                }

                return Redirect(string.Format("/Menu/ViewMenu/{0}", addMenuItemViewModel.MenuID));

            }

            return View(addMenuItemViewModel);
        }
    }
}

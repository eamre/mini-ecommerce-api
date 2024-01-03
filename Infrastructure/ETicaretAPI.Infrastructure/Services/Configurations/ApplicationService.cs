using ETicaretAPI.Application.Abstractions.Services.Configurations;
using ETicaretAPI.Application.CustomAttributes;
using ETicaretAPI.Application.DTOs.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Infrastructure.Services.Configurations
{
    public class ApplicationService : IApplicationService
    {
        public List<Menu> GetAuthorizeDefinitionEndPoints(Type type)
        {
            Assembly assembly = Assembly.GetAssembly(type);
            var controllers = assembly.GetTypes().Where(t => t.IsAssignableTo(typeof(ControllerBase)));// ControllerBase sınıfından türeyen sınıfları getir
            var menus = new List<Menu>(); 

            if (controllers != null)
            {
                foreach (var controller in controllers)
                {
                    var actions = controller.GetMethods().Where(m => m.IsDefined(typeof(AuthorizeDefinitionAttribute)));// metot üzerindeki AuthorizeDefinitionAttribute özelliği varsa o metodu al
                    
                    if (actions != null)
                    {
                        foreach (var action in actions)
                        {
                            var attributes = action.GetCustomAttributes(true);
                            if (attributes != null)
                            {
                                Menu? menu = null;
                                var authorizeDefinitionAttribute = attributes.FirstOrDefault(a => a.GetType() == typeof(AuthorizeDefinitionAttribute)) as AuthorizeDefinitionAttribute;
                               
                                if (!menus.Any(m => m.Name == authorizeDefinitionAttribute?.Menu)) {
                                    menu = new Menu()
                                    {
                                        Name = authorizeDefinitionAttribute.Menu,
                                    };
                                    menus.Add(menu);
                                }
                                else
                                    menu = menus.FirstOrDefault(m => m.Name == authorizeDefinitionAttribute.Menu);

                                var httpType = action.GetCustomAttribute<HttpMethodAttribute>()?.HttpMethods.FirstOrDefault();

                                Application.DTOs.Configuration.Action _action = new()
                                {
                                    ActionType = authorizeDefinitionAttribute.ActionType.ToString(),
                                    Definition = authorizeDefinitionAttribute.Definition,
                                    HttpType = httpType
                                };

                                _action.Code = $"{_action.HttpType}.{_action.ActionType}.{_action.Definition.Replace(" ", "")}";

                                menu?.Actions.Add(_action);
                            }
                        }
                    }
                }
            }

            return menus;
        }
    }
}

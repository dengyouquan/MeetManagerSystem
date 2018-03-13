using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;

namespace BLL
{
    public class BLLContainer
    {
        public static IContainer container = null;
        public static T Resolve<T>()
        {
            try
            {
                if (container == null)
                {
                    Initialise();
                }
            }
            catch (Exception e)
            {
                throw new Exception("IOC容器初始化出错" + e.Message);
            }
            return container.Resolve<T>();
        }

        private static void Initialise()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<MeetService>().As<IMeetService>().InstancePerLifetimeScope();
            builder.RegisterType<ExamineService>().As<IExamineService>().InstancePerLifetimeScope();
            builder.RegisterType<MeetMsgService>().As<IMeetMsgService>().InstancePerLifetimeScope();
            builder.RegisterType<MeetRoomService>().As<IMeetRoomService>().InstancePerLifetimeScope();
            container = builder.Build();
        }
    }
}

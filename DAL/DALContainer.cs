using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;

namespace DAL
{
    public class DALContainer
    {
        /// <summary>
        /// IOC容器(也可以自己写一个工厂方法)
        /// </summary>
        public static IContainer container = null;
        /// <summary>
        /// 获取Idal的实例化对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
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
                throw new Exception("IOC实例化出错" + e.Message);
            }
            return container.Resolve<T>();
        }

        private static void Initialise()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<MeetDAL>().As<IMeetDAL>().InstancePerLifetimeScope();
            builder.RegisterType<ExamineDAL>().As<IExamineDAL>().InstancePerLifetimeScope();
            builder.RegisterType<MeetMsgDAL>().As <IMeetMsgDAL>().InstancePerLifetimeScope();
            builder.RegisterType<MeetRoomDAL>().As<IMeetRoomDAL>().InstancePerLifetimeScope();
            container = builder.Build();
        }
    }
}

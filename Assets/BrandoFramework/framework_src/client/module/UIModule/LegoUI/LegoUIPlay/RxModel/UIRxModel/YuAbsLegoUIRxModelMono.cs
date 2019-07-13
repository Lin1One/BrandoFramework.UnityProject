//#region Head

//// Author:            Yu
//// CreateDate:        2018/8/31 10:59:33
//// Email:             35490136@qq.com

///*
// * 修改日期  ：
// * 修改人    ：
// * 修改内容  ：
//*/

//#endregion

//using System.Collections.Generic;
//using UnityEngine;
//using YuPlay;

//namespace YuLegoUIPlay
//{
//    public abstract class YuAbsLegoUIRxModelMono :
//        MonoBehaviour,
//        IYuLegoUIRxModel
//    {
//        private readonly YuLegoUIRxModel modelProxy
//            = new YuLegoUIRxModel();

//        public T GetControlRxModel<T>(string id) where T : class, IYuRelease
//        {
//            return modelProxy.GetControlRxModel<T>(id);
//        }

//        public Dictionary<string, object> Models => modelProxy.Models;

//        public abstract void InitRxModel();
//        public IYuLegoUI LocUI { get; set; }

//        public void Copy(IYuLegoUIRxModel target) => modelProxy.Copy(target);

//        protected T AddControlRxModel<T>(string id) where T : class, IYuRelease
//        {
//            return modelProxy.AddControlRxModel<T>(id);
//        }

//        public void Release()
//        {
//            modelProxy.Release();
//        }

//        public void CopyFromJson()
//        {
//            throw new System.NotImplementedException();
//        }
//    }
//}
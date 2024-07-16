using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EggCard;
using UnityEngine;

namespace HotUpdate
{
    public class ProcedureComponent : MonoSingleton<ProcedureComponent>
    {
        ProcedureMgr _procedureMgr;
        
        [SerializeField]
        private string[] m_AvailableProcedureTypeNames = null;
        
        [SerializeField]
        private string m_EntranceProcedureTypeName = null;
        
        private ProcedureBase m_EntranceProcedure = null;

        private void Start()
        {
            Init();
        }

        private void Update()
        {
            _procedureMgr.Update();
        }

        private void OnDestroy()
        {
            _procedureMgr.Destroy();
        }

        /// <summary>
        /// 获取当前流程。
        /// </summary>
        public ProcedureBase CurrentProcedure
        {
            get
            {
                return _procedureMgr.GetCurrentState();
            }
        }

        private void Init()
        {
            List<ProcedureBase> procedures = new List<ProcedureBase>();

            foreach (var procedure in m_AvailableProcedureTypeNames)
            {

                Assembly assembly = GetType().Assembly;

                Type procedureType = assembly.GetType(procedure);
                
                if (procedureType == null)
                {
                    Debug.LogError("Can not find procedure type " + procedure);
                    return;
                }
                procedures.Add((ProcedureBase)Activator.CreateInstance(procedureType));
                if (procedures.Last() == null)
                {
                    Debug.LogError("Can not create procedure instance" + procedure);
                    return;
                }
                if (m_EntranceProcedureTypeName == procedure)
                {
                    m_EntranceProcedure = procedures.Last();
                }
            }
            
            _procedureMgr = new ProcedureMgr();
            _procedureMgr.InitFsm(procedures.ToArray());
            _procedureMgr.Start(m_EntranceProcedure.GetType());
        }
    }
}
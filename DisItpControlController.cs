using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using NavesPortalforWebWithCoreMvc.Common;
using NavesPortalforWebWithCoreMvc.Controllers.AuthFromIntranetController;
using NavesPortalforWebWithCoreMvc.Controllers.Uploader;
using NavesPortalforWebWithCoreMvc.Data;
using NavesPortalforWebWithCoreMvc.Models;
using NavesPortalforWebWithCoreMvc.RfSystemData;
using Syncfusion.EJ2.Base;
using System.Collections;

namespace NavesPortalforWebWithCoreMvc.Controllers.DIS
{
    [Authorize]
    [CheckSession]
    public class DisItpControlController : Controller
    {
        private readonly BM_NAVES_PortalContext _repository;
        private readonly IBM_NAVES_PortalContextProcedures _procedure;

        public DisItpControlController(BM_NAVES_PortalContext repository, IBM_NAVES_PortalContextProcedures procedure)
        {
            _repository = repository;
            _procedure = procedure;
        }

        /// <summary>
        /// ITP Control List
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Index()
        {
            return await Task.Run(() => View());
        }

        /// <summary>
        /// 목록
        /// </summary>
        /// <param name="SearchString"></param>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <param name="dm"></param>
        /// <returns></returns>
        public async Task<IActionResult> UrlDataSource(string SearchString, DateTime? StartDate, DateTime? EndDate, [FromBody] DataManagerRequest? dm)
        {
            try
            {
                if (SearchString is null || SearchString == String.Empty)
                {
                    SearchString = "";
                }

                List<PNAV_DIS_GET_ITP_CONTROL_WORK_LISTResult> resultList = await _procedure.PNAV_DIS_GET_ITP_CONTROL_WORK_LISTAsync(SearchString.Trim(), StartDate, EndDate);

                IEnumerable DataSource = resultList.AsEnumerable();
                DataOperations operation = new DataOperations();

                //Search
                if (dm.Search != null && dm.Search.Count > 0)
                {
                    DataSource = operation.PerformSearching(DataSource, dm.Search);
                }

                if (dm.Sorted != null && dm.Sorted.Count > 0) //Sorting
                {
                    DataSource = operation.PerformSorting(DataSource, dm.Sorted);
                }

                //Filtering
                if (dm.Where != null && dm.Where.Count > 0)
                {
                    DataSource = operation.PerformFiltering(DataSource, dm.Where, dm.Where[0].Operator);
                }

                int count = DataSource.Cast<PNAV_DIS_GET_ITP_CONTROL_WORK_LISTResult>().Count();

                //Paging
                if (dm.Skip != 0)
                {

                    DataSource = operation.PerformSkip(DataSource, dm.Skip);
                }

                if (dm.Take != 0)
                {
                    DataSource = operation.PerformTake(DataSource, dm.Take);
                }

                return dm.RequiresCounts ? Json(new { result = DataSource, count = count }) : Json(new { result = DataSource });
            }
            catch (Exception e)
            {
                return RedirectToAction("SaveException", "Error", new { ex = e.InnerException.Message, returnController = "DisItpControl", returnView = "Index" });
            }
        }

        /// <summary>
        /// WORI_ID 별 ITP 목록
        /// </summary>
        /// <param name="id">WORI_ID</param>
        /// <returns></returns>
        public async Task<IActionResult> Detail(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Work ID
            ViewBag.dataSource = await _procedure.PNAV_DIS_GET_WORK_ID_INFOAsync(id);

            // ITP List           
            List<PNAV_DIS_GET_ITP_LIST_IN_WORKResult> itpLIst = await _procedure.PNAV_DIS_GET_ITP_LIST_IN_WORKAsync(
                    id.ToString(),
                    CommonSettingData.Dis_Itp_Status.CONFIRM.ToString(),
                    CommonSettingData.Dis_Action_Status.APPLY.ToString(),
                    "");

            ViewBag.WorkId = id;

            if (itpLIst.Count() <= 0)
            {
                ViewBag.Msg = "<ul><li>[ITP] Arrangement: [NOT] exists. </li><li>You must create an ITP for this Work-ID.</li></ul>";
                ViewBag.IsShowModal = true;
            }
            else
            {
                ViewBag.Msg = string.Empty;
                ViewBag.IsShowModal = false;
            }

            return View();
        }

        /// <summary>
        /// Work ID 별 ITP List
        /// </summary>
        /// <param name="WorkID"></param>
        /// <param name="ItpStatus"></param>
        /// <param name="ActionStatus"></param>
        /// <param name="SearchString"></param>
        /// <param name="dm"></param>
        /// <returns></returns>
        public async Task<IActionResult> UrlDataSourceItpList(string WorkID, string ItpStatus, string ActionStatus, string? SearchString, [FromBody] DataManagerRequest? dm)
        {
            try
            {
                if (SearchString is null || SearchString == String.Empty)
                {
                    SearchString = "";
                }

                List<PNAV_DIS_GET_ITP_LIST_IN_WORKResult> resultList = await _procedure.PNAV_DIS_GET_ITP_LIST_IN_WORKAsync(WorkID.ToString(), ItpStatus, ActionStatus, SearchString);

                IEnumerable DataSource = resultList.AsEnumerable();
                DataOperations operation = new DataOperations();

                //Search
                if (dm.Search != null && dm.Search.Count > 0)
                {
                    DataSource = operation.PerformSearching(DataSource, dm.Search);
                }

                if (dm.Sorted != null && dm.Sorted.Count > 0) //Sorting
                {
                    DataSource = operation.PerformSorting(DataSource, dm.Sorted);
                }

                //Filtering
                if (dm.Where != null && dm.Where.Count > 0)
                {
                    DataSource = operation.PerformFiltering(DataSource, dm.Where, dm.Where[0].Operator);
                }

                int count = DataSource.Cast<PNAV_DIS_GET_ITP_LIST_IN_WORKResult>().Count();

                //Paging
                if (dm.Skip != 0)
                {

                    DataSource = operation.PerformSkip(DataSource, dm.Skip);
                }

                if (dm.Take != 0)
                {
                    DataSource = operation.PerformTake(DataSource, dm.Take);
                }

                return dm.RequiresCounts ? Json(new { result = DataSource, count = count }) : Json(new { result = DataSource });
            }
            catch (Exception e)
            {
                return RedirectToAction("SaveException", "Error", new { ex = e.InnerException.Message, returnController = "DisItpControl", returnView = "Index" });
            }
        }

        /// <summary>
        /// Itp Control Revision 중인 itp List
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> UrlDataSourceItpRevisionList(Guid? id)
        {
            List<PNAV_GET_DIS_ITP_CONTROL_REVISION_LISTResult> itpRevisionList = await _procedure.PNAV_GET_DIS_ITP_CONTROL_REVISION_LISTAsync(id);

            return Json(new
            {
                result = itpRevisionList,
                count = itpRevisionList.Count()
            });
        }

        public async Task<IActionResult> UrlDataSourceMasterItp(string CODE)
        {
            List<VNAV_SELECT_DIS_ITP_MASTER_LIST> result = await _repository.VNAV_SELECT_DIS_ITP_MASTER_LISTs
                                                            .Where(m => m.IS_DELETED == false
                                                                        && m.CODE.Contains(CODE)
                                                                        && m.PART.Contains(CODE)
                                                                        && m.ITEM.Contains(CODE)
                                                                        && m.INSPECTION.Contains(CODE))
                                                            .ToListAsync();
            return Json(result);
        }

        /// <summary>
        /// WORI_ID
        /// </summary>
        /// <param name="id">WORI_ID</param>
        /// <returns></returns>
        public async Task<IActionResult> Create(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ViewBag.title = "ITP Control New/Edit";

            List<VNAV_SELECT_DIS_ITP_WORK_LIST> workList = await _repository.VNAV_SELECT_DIS_ITP_WORK_LISTs.Where(m => m.WORK_IDX == id).ToListAsync();
            List<PNAV_DIS_GET_ITP_LIST_IN_WORKResult> itpLIst = await _procedure.PNAV_DIS_GET_ITP_LIST_IN_WORKAsync(
                                                                        id.ToString(), "", CommonSettingData.Dis_Action_Status.REVISION.ToString(), "");

            var query = _repository.TNAV_DIS_ITP_CONTROLs
                                    .Where(m => m.WORK_IDX == id
                                         && m.ITP_STATUS == CommonSettingData.Dis_Itp_Status.CONFIRM.ToString()
                                         && m.ACTION_STATUS == CommonSettingData.Dis_Action_Status.APPLY.ToString())
                                    .AsQueryable();

            // currnet Itp 수
            var _itpCnt = await query.CountAsync();

            //current Revision No.            
            int _revisionNo = query?.Max(m => m.ITP_VERSION) + 1 ?? 0;

            string status = GetItpRevisionStatus(id, _revisionNo);
            ViewBag.Status = status;
            ViewBag.RevisionNo = _revisionNo;
            //ViewBag.Revision = "New Revision : " + _revisionNo.ToString();
            ViewBag.ItpStatus = status;
            ViewBag.dataSource = workList;

            //ViewBag.dataSourceItpList = itpLIst;

            ViewBag.IsItpCnt = _itpCnt;
            ViewBag.RevisionCount = itpLIst.Count();
            ViewBag.WorkId = workList[0].WORK_ID;
            ViewBag.MasterItp = _repository.VNAV_SELECT_DIS_ITP_MASTER_LISTs.Where(m => m.IS_DELETED == false).ToList();

            // 새롭게 생성되는 ITP Control에 사용되는 ITP_IDX
            // 신규 IPT 추가에서만 사용 됨.
            //ViewBag.CurrentIdx = Guid.NewGuid(); 
            ViewBag.CreateUser = HttpContext.Session.GetString("UserName");

            List<dropdownViewModel> ddlType = new List<dropdownViewModel>();
            ddlType.Add(new dropdownViewModel { Name = "Select Type...", Value = "" });
            ddlType.Add(new dropdownViewModel { Name = "Common", Value = "COMMON" });
            ddlType.Add(new dropdownViewModel { Name = "Type A", Value = "TYPE_A" });
            ddlType.Add(new dropdownViewModel { Name = "Type B", Value = "TYPE_B" });
            ddlType.Add(new dropdownViewModel { Name = "Type C", Value = "TYPE_C" });
            ddlType.Add(new dropdownViewModel { Name = "Type D", Value = "TYPE_D" });
            ddlType.Add(new dropdownViewModel { Name = "Type E", Value = "TYPE_E" });
            ViewBag.Type = ddlType;

            ViewBag.part = _repository.VNAV_SELECT_DIS_CODE_PART_LISTs.ToList();
            ViewBag.group = _repository.VNAV_SELECT_DIS_CODE_GROUP_LISTs.ToList();
            ViewBag.Item = _repository.VNAV_SELECT_DIS_CODE_ITEM_LISTs.ToList();

            TNAV_DIS_ITP_CONTROL itp = new TNAV_DIS_ITP_CONTROL()
            {
                // 새롭게 생성되는 ITP Control에 사용되는 ITP_IDX                
                PROJECT_IDX = workList[0].PROJECT_IDX,
                PROJECT_ID = workList[0].PROJECT_ID,
                WORK_IDX = workList[0].WORK_IDX,
                WORK_ID = workList[0].WORK_ID,
            };
            ViewBag.TempItpIdx = Guid.NewGuid().ToString().ToUpper();
            ViewBag.FileList = new List<TNAV_COM_FILE>();

            return View(itp);
        }

        public string GetItpRevisionStatus(Guid? id, int revisionNo)
        {
            return _repository.TNAV_DIS_ITP_CONTROLs.Where(m => m.WORK_IDX == id && m.ITP_VERSION == revisionNo).FirstOrDefault()?.ITP_STATUS ?? "Empty";
        }

        /// <summary>
        /// ITP 상세
        /// </summary>
        /// <param name="ITP_IDX"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GetItpDetail(Guid ITP_IDX)
        {
            TNAV_DIS_ITP_CONTROL _ITP_Detail = await _repository.TNAV_DIS_ITP_CONTROLs.Where(m => m.ITP_IDX == ITP_IDX).FirstAsync();
            return Json(new
            {
                result = "OK",
                status = _ITP_Detail.ITP_STATUS,
                version = _ITP_Detail.ITP_VERSION,
                data = _ITP_Detail
            });
        }

        /// <summary>
        /// ITP 생성(리비전)후 GM 승인 요청
        /// </summary>
        /// <param name="workId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ItpSubmit(Guid? workIdx)
        {
            try
            {
                // max rev. No 확인
                List<TNAV_DIS_ITP_CONTROL> _targetSubmit = null;
                int _maxRevisionNo = _repository.TNAV_DIS_ITP_CONTROLs.Where(m => m.WORK_IDX == workIdx).Max(m => m.ITP_VERSION) ?? 0;
                if (_maxRevisionNo == 0)
                {
                    // max rev. 가 0이면, ITP Status 가 NEW 인 목록
                    _targetSubmit = await _repository.TNAV_DIS_ITP_CONTROLs
                       .Where(m => m.WORK_IDX == workIdx
                           && m.ITP_STATUS == CommonSettingData.Dis_Itp_Status.NEW.ToString()
                           && m.ITP_VERSION == 0).ToListAsync();
                }
                else
                {
                    // max rev. 가 0이 아니면 Action Status 가 REVISION 이고 ITP Status 가 NEW인 목록
                    _targetSubmit = await _repository.TNAV_DIS_ITP_CONTROLs
                       .Where(m => m.WORK_IDX == workIdx
                           && m.ITP_STATUS == CommonSettingData.Dis_Itp_Status.NEW.ToString()
                           && m.ACTION_STATUS == CommonSettingData.Dis_Action_Status.REVISION.ToString()
                           && m.ITP_VERSION == _maxRevisionNo).ToListAsync();
                }

                foreach (TNAV_DIS_ITP_CONTROL itp in _targetSubmit)
                {
                    itp.ITP_STATUS = CommonSettingData.Dis_Itp_Status.SUBMIT.ToString();
                }

                // 상태값 저장
                TNAV_COMMON_LOG LogViewModel = new TNAV_COMMON_LOG()
                {
                    REG_DATE = DateTime.Now,
                    USER_NAME = HttpContext.Session.GetString("UserName"),
                    PLATFORM = "DIS",
                    MENU_NAME = "DIS ITP Control",
                    TARGET_IDX = workIdx,
                    STATUS = CommonSettingData.LogStatus.SUBMIT.ToString()
                };
                _repository.Add(LogViewModel);

                _repository.UpdateRange(_targetSubmit);
                _repository.SaveChanges();

                return Json(new
                {
                    result = "OK",
                    Work_ID = workIdx
                });
            }
            catch (Exception)
            {
                return Json(new
                {
                    result = "Error",
                    Work_ID = workIdx
                });
                throw;
            }
        }

        /// <summary>
        /// ITP 생성(리비전)에 대한 승인요청 후 확인 
        /// </summary>
        /// <param name="workId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ItpConfirmByGm(Guid? workIdx)
        {
            // max rev. No 확인
            List<TNAV_DIS_ITP_CONTROL> _targetSubmit = null;
            int _maxRevisionNo = _repository.TNAV_DIS_ITP_CONTROLs.Where(m => m.WORK_IDX == workIdx).Max(m => m.ITP_VERSION) ?? 0;
            if (_maxRevisionNo == 0)
            {
                // max rev. 가 0이면, ITP Status 가 NEW 인 목록
                _targetSubmit = await _repository.TNAV_DIS_ITP_CONTROLs
                   .Where(m => m.WORK_IDX == workIdx
                       && m.ITP_STATUS == CommonSettingData.Dis_Itp_Status.SUBMIT.ToString()
                       && m.ITP_VERSION == 0).ToListAsync();
            }
            else
            {
                // max rev. 가 0이 아니면 Action Status 가 REVISION 이고 ITP Status 가 NEW인 목록
                _targetSubmit = await _repository.TNAV_DIS_ITP_CONTROLs
                   .Where(m => m.WORK_IDX == workIdx
                       && m.ITP_STATUS == CommonSettingData.Dis_Itp_Status.SUBMIT.ToString()
                       && m.ACTION_STATUS == CommonSettingData.Dis_Action_Status.REVISION.ToString()
                       && m.ITP_VERSION == _maxRevisionNo).ToListAsync();
            }

            foreach (TNAV_DIS_ITP_CONTROL itp in _targetSubmit)
            {
                itp.ITP_STATUS = CommonSettingData.Dis_Itp_Status.CONFIRM.ToString();
                itp.ACTION_STATUS = CommonSettingData.Dis_Action_Status.APPLY.ToString();
            }

            // 상태값 저장
            TNAV_COMMON_LOG LogViewModel = new TNAV_COMMON_LOG()
            {
                REG_DATE = DateTime.Now,
                USER_NAME = HttpContext.Session.GetString("UserName"),
                PLATFORM = "DIS",
                MENU_NAME = "DIS ITP Control",
                TARGET_IDX = workIdx,
                STATUS = CommonSettingData.LogStatus.CONFIRM.ToString()
            };
            _repository.Add(LogViewModel);

            _repository.UpdateRange(_targetSubmit);
            _repository.SaveChanges();
            return Json(new
            {
                result = "OK",
                Work_ID = workIdx
            });
        }

        /// <summary>
        /// ITP Revision 생성
        /// </summary>
        /// <param name="workId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ItpCreateRevision(Guid? workIdx)
        {
            try
            {
                var query = _repository.TNAV_DIS_ITP_CONTROLs
                                    .Where(m => m.WORK_IDX == workIdx
                                         && m.ITP_STATUS == CommonSettingData.Dis_Itp_Status.CONFIRM.ToString()
                                         && m.ACTION_STATUS == CommonSettingData.Dis_Action_Status.APPLY.ToString())
                                    .AsQueryable();

                // 적용된 ITP가 있는 상태여야 함
                if (await query.AnyAsync())
                {
                    // 적용된 IPT 버전                    
                    int _revisionNo = query?.Max(m => m.ITP_VERSION) ?? 0;

                    // 현재 IPT 목록
                    List<TNAV_DIS_ITP_CONTROL> itpLIst = await query.Where(m => m.REVISION_STATUS != CommonSettingData.Dis_Revision_Status.DELETE.ToString()
                                                                                && m.ITP_VERSION == _revisionNo
                                                                                && m.OUT_OF_USE == false
                                                                                && m.IS_DELETED == false)
                                                                    .AsNoTracking()
                                                                    .OrderBy(m => m.ITP_NO)
                                                                    .ToListAsync();

                    List<TNAV_DIS_ITP_CONTROL> revisionItp = new List<TNAV_DIS_ITP_CONTROL>();
                    List<TNAV_COM_FILE> NewItpFiles = new List<TNAV_COM_FILE>();

                    // Revision 추가
                    foreach (TNAV_DIS_ITP_CONTROL itp in itpLIst)
                    {
                        Guid OldItpIdx = itp.ITP_IDX;

                        itp.ITP_IDX = Guid.NewGuid();
                        itp.ITP_VERSION = _revisionNo + 1;
                        itp.REVISION_STATUS = itp.REVISION_STATUS;
                        itp.ITP_STATUS = CommonSettingData.Dis_Itp_Status.NEW.ToString();
                        itp.ACTION_STATUS = CommonSettingData.Dis_Action_Status.REVISION.ToString();
                        itp.CREATE_USER_NAME = HttpContext.Session.GetString("UserName");
                        itp.REG_DATE = DateTime.Now;
                        itp.REMARK = string.Format($"{itp.REMARK}<br/><p>- Revision {_revisionNo + 1} Created: {DateTime.Now.ToString()} by {HttpContext.Session.GetString("UserName")}</p>");

                        //revisionItp.Add(itp);

                        // 1. ITP Master Code가 있는 ITP면 Survey Procedure fiel 정보를 신규로 등록한다.
                        if (itp.CODE != null && itp.CODE != "")
                        {
                            // ITP Master에 등록되어 있는 File 정보를 확인해 TNAV_COM_FILE에 정보만 저장
                            // 1. CODE를 이용해 ITP Master ITP_IDX를 조회
                            Guid _itp_Idx = _repository.TNAV_DIS_ITP_MASTERs.Where(m => m.CODE == itp.CODE).Select(m => m.ITP_IDX).FirstOrDefault();

                            // 2. ITP Master ITP_IDX를 이용해 COM_FILES 의 정보 조회
                            List<TNAV_COM_FILE> fileList = _repository.TNAV_COM_FILEs.Where(m => m.DOCUMENT_IDX.Equals(_itp_Idx)).AsNoTracking().ToList();

                            // 3. IPT Master에서 업로드된 파일이 ITP Master에서 삭제되면 파일을 확인 할 수 없어 서버에 동일한 파일을 파일명을 변경해 복사하기
                            // 구현 필요

                            // 4. 신규 ITP에 조회한 ITP Master Com_File 정보를 저장
                            Guid _idx = Guid.NewGuid();
                            foreach (TNAV_COM_FILE file in fileList)
                            {
                                file.IDX = Guid.NewGuid();
                                string DestinationFullPath = await FileHandler.ChangeSavedFullPath(file.SAVED_FULL_PATH, itp.ITP_IDX, "DIS", itp.PROJECT_ID, itp.WORK_ID);
                                string DestinationFileName = await FileHandler.ChangeSavedFileName(DestinationFullPath);

                                file.DOCUMENT_IDX = itp.ITP_IDX;
                                file.KIND_OF_FILES = "ITP_CONTROL";
                                file.RELATED_INFO = _itp_Idx.ToString();
                                file.PROJECT_IDX = itp.PROJECT_IDX;
                                file.PROJECT_ID = itp.PROJECT_ID;
                                file.WORK_IDX = itp.WORK_IDX;
                                file.WORK_ID = itp.WORK_ID;
                                file.REG_DATE = DateTime.Now;
                                file.CREATE_USER_NAME = HttpContext.Session.GetString("UserName");
                                file.SAVED_FULL_PATH = DestinationFullPath;
                                file.SAVED_FILENAME = DestinationFileName;

                                NewItpFiles.Add(file);
                            }
                        }
                        else
                        {
                            // 1. TYPE을 이용해 ITP Master ITP_IDX List를 조회
                            // 2. ITP Master ITP_IDX를 이용해 COM_FILES 의 정보 조회
                            List<TNAV_COM_FILE> fileList = _repository.TNAV_COM_FILEs.Where(m => m.DOCUMENT_IDX.Equals(OldItpIdx)).AsNoTracking().ToList();
                            

                            foreach (TNAV_COM_FILE file in fileList)
                            {
                                file.IDX = Guid.NewGuid();

                                string DestinationFullPath = await FileHandler.ChangeSavedFullPath(file.SAVED_FULL_PATH, itp.ITP_IDX, "DIS", itp.PROJECT_ID, itp.WORK_ID);
                                string DestinationFileName = await FileHandler.ChangeSavedFileName(DestinationFullPath);

                                file.DOCUMENT_IDX = itp.ITP_IDX;
                                file.KIND_OF_FILES = "ITP_CONTROL";
                                file.RELATED_INFO = String.Empty;
                                file.PROJECT_IDX = itp.PROJECT_IDX;
                                file.PROJECT_ID = itp.PROJECT_ID;
                                file.WORK_IDX = itp.WORK_IDX;
                                file.WORK_ID = itp.WORK_ID;
                                file.REG_DATE = DateTime.Now;
                                file.CREATE_USER_NAME = HttpContext.Session.GetString("UserName");
                                file.SAVED_FULL_PATH = DestinationFullPath;
                                file.SAVED_FILENAME = DestinationFileName;

                                NewItpFiles.Add(file);
                            }
                        }

                        revisionItp.Add(itp);
                    }


                    await _repository.AddRangeAsync(NewItpFiles);
                    await _repository.AddRangeAsync(revisionItp);
                    await _repository.SaveChangesAsync();

                    return Json(new
                    {
                        result = "OK",
                        Work_ID = workIdx
                    });
                }
                else
                {
                    return Json(new
                    {
                        result = "Error",
                        Work_ID = workIdx
                    });
                }
            }
            catch (Exception e)
            {
                return RedirectToAction("SaveException", "Error", new { ex = e.InnerException.Message, returnController = "DisItpControl", returnView = "Create" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetItpMasterItem(string Code)
        {
            try
            {
                TNAV_DIS_ITP_MASTER _ITP_MASTER = await _repository.TNAV_DIS_ITP_MASTERs.Where(m => m.CODE == Code).FirstAsync();
                return Json(new
                {
                    data = _ITP_MASTER,
                    result = "OK"
                });
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 선택된 ITP Master에 첨부된 파일 정보 가져오기
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="KindOfFiles"></param>
        /// <returns></returns>
        public async Task<IActionResult> PartialFileList(Guid Id, string KindOfFiles)
        {
            // 첨부파일 목록
            ViewBag.FileList = await _repository.TNAV_COM_FILEs.Where(m => m.DOCUMENT_IDX == Id && m.KIND_OF_FILES == KindOfFiles).OrderByDescending(m => m.REG_DATE).ToListAsync();

            return PartialView("_Pv_UploadedFileList", ViewBag.FileList);
        }

        /// <summary>
        /// 개별 ITP 등록
        /// </summary>
        /// <param name="newItp"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveNewItp(TNAV_DIS_ITP_CONTROL newItp, Guid TempItpIdx)
        {
            try
            {
                if (newItp != null)
                {
                    TNAV_DIS_ITP_CONTROL _DIS_ITP_CONTROL;
                    List<TNAV_DIS_ITP_CONTROL> ItpControlMasterList = new List<TNAV_DIS_ITP_CONTROL>();
                    int _revisionNo = _repository.TNAV_DIS_ITP_CONTROLs.Where(m => m.WORK_IDX == newItp.WORK_IDX).Max(m => m.ITP_VERSION) ?? 0;

                    string _REVISION_STATUS = String.Empty;
                    if (_revisionNo == 0)
                    {
                        _REVISION_STATUS = CommonSettingData.Dis_Revision_Status.ORIGINAL.ToString();
                    }
                    else
                    {
                        _REVISION_STATUS = CommonSettingData.Dis_Revision_Status.ADD.ToString();
                    }

                    // Type 이 있으면
                    if (newItp.TYPE != null)
                    {
                        // Use Type을 이용한 생성
                        if (_revisionNo == 0)
                        {
                            // Revision 0일때는 ITP Master에서 불러와 저장
                            // Vessel Type 불러오기
                            List<PNAV_GET_DIS_ITP_MASTER_TYPE_LISTResult> typeList = await _procedure.PNAV_GET_DIS_ITP_MASTER_TYPE_LISTAsync(newItp.TYPE);

                            int _itpNo = 1;
                            foreach (PNAV_GET_DIS_ITP_MASTER_TYPE_LISTResult item in typeList)
                            {
                                Guid _itp = Guid.NewGuid();
                                ItpControlMasterList.Add(new TNAV_DIS_ITP_CONTROL()
                                {
                                    ITP_IDX = _itp,
                                    ITP_NO = _itpNo,
                                    PROJECT_IDX = newItp.PROJECT_IDX,
                                    PROJECT_ID = newItp.PROJECT_ID,
                                    WORK_IDX = newItp.WORK_IDX,
                                    WORK_ID = newItp.WORK_ID,
                                    CODE = item.CODE,
                                    TYPE = newItp.TYPE,
                                    PART = getCodeName(item.PART_CODE, item.GROUP_CODE, item.ITEM_CODE, "PART"),
                                    PART_CODE = item.PART_CODE,
                                    GROUP = getCodeName(item.PART_CODE, item.GROUP_CODE, item.ITEM_CODE, "GROUP"),
                                    GROUP_CODE = item.GROUP_CODE,
                                    ITEM = getCodeName(item.PART_CODE, item.GROUP_CODE, item.ITEM_CODE, "ITEM"),
                                    ITEM_CODE = item.ITEM_CODE,
                                    INSPECTION = item.INSPECTION,
                                    ITP_STATUS = CommonSettingData.Dis_Itp_Status.NEW.ToString(),
                                    REVISION_STATUS = _REVISION_STATUS,
                                    ACTION_STATUS = CommonSettingData.Dis_Action_Status.REVISION.ToString(),
                                    CLIENT_REFERENCE_NO = newItp.CLIENT_REFERENCE_NO,
                                    REMARK = newItp.REMARK,
                                    OUT_OF_USE = false,
                                    REG_DATE = DateTime.Now,
                                    CREATE_USER_NAME = HttpContext.Session.GetString("UserName"),
                                    ITP_VERSION = 0
                                });

                                _itpNo++;

                                // ITP Master에 등록되어 있는 File 정보를 확인해 TNAV_COM_FILE에 정보 저장
                                // 1. TYPE을 이용해 ITP Master ITP_IDX List를 조회
                                // 2. ITP Master ITP_IDX를 이용해 COM_FILES 의 정보 조회
                                List<TNAV_COM_FILE> fileList = _repository.TNAV_COM_FILEs.Where(m => m.DOCUMENT_IDX.Equals(item.ITP_IDX)).AsNoTracking().ToList();

                                // 3. 신규 ITP에 조회한 ITP Master Com_File 정보를 저장
                                List<TNAV_COM_FILE> NewItpFiles = new List<TNAV_COM_FILE>();
                                Guid _idx = Guid.NewGuid();
                                foreach (TNAV_COM_FILE file in fileList)
                                {
                                    file.IDX = Guid.NewGuid();
                                    string DestinationFullPath = await FileHandler.ChangeSavedFullPath(file.SAVED_FULL_PATH, _idx, "DIS", newItp.PROJECT_ID, newItp.WORK_ID);
                                    string DestinationFileName = await FileHandler.ChangeSavedFileName(DestinationFullPath);

                                    file.DOCUMENT_IDX = _itp;
                                    file.KIND_OF_FILES = "ITP_CONTROL";
                                    file.RELATED_INFO = item.ITP_IDX.ToString();
                                    file.PROJECT_IDX = newItp.PROJECT_IDX;
                                    file.PROJECT_ID = newItp.PROJECT_ID;
                                    file.WORK_IDX = newItp.WORK_IDX;
                                    file.WORK_ID = newItp.WORK_ID;
                                    file.REG_DATE = DateTime.Now;
                                    file.CREATE_USER_NAME = HttpContext.Session.GetString("UserName");
                                    file.SAVED_FULL_PATH = DestinationFullPath;
                                    file.SAVED_FILENAME = DestinationFileName;

                                    NewItpFiles.Add(file);
                                }

                                // IPT Master의 파일 정보 저장
                                await _repository.AddRangeAsync(NewItpFiles);
                            }

                            await _repository.AddRangeAsync(ItpControlMasterList);

                            // 변경된 ITP Master vessel type이면 기존에 등록된 모든 IPT List 삭제
                            List<TNAV_DIS_ITP_CONTROL> targetRemoveList = _repository.TNAV_DIS_ITP_CONTROLs.Where(m => m.WORK_IDX == newItp.WORK_IDX).ToList();
                            _repository.RemoveRange(targetRemoveList);
                        }
                        else
                        {
                            // Type을 이용한 추가는 Revision 0에서만 가능
                            // 관련 기능 추가
                            return RedirectToAction(nameof(Create), new { id = newItp.WORK_IDX });
                        }
                    }
                    else
                    {
                        // Master ITP 한개 추가
                        // ITP 고유번호 부여
                        int _itpNo = 1;
                        if (_repository.TNAV_DIS_ITP_CONTROLs.Where(m => m.WORK_IDX == newItp.WORK_IDX).Count() > 0)
                        {
                            _itpNo = _repository.TNAV_DIS_ITP_CONTROLs.Where(m => m.WORK_IDX == newItp.WORK_IDX).Max(m => m.ITP_NO) + 1;
                        }

                        // Revision 1이상일때
                        _DIS_ITP_CONTROL = new TNAV_DIS_ITP_CONTROL()
                        {
                            ITP_IDX = TempItpIdx,
                            ITP_NO = _itpNo,
                            PROJECT_IDX = newItp.PROJECT_IDX,
                            PROJECT_ID = newItp.PROJECT_ID,
                            WORK_IDX = newItp.WORK_IDX,
                            WORK_ID = newItp.WORK_ID,
                            CODE = newItp.CODE,
                            TYPE = String.Empty,
                            PART = getCodeName(newItp.PART_CODE, newItp.GROUP_CODE, newItp.ITEM_CODE, "PART"),
                            PART_CODE = newItp.PART_CODE,
                            GROUP = getCodeName(newItp.PART_CODE, newItp.GROUP_CODE, newItp.ITEM_CODE, "GROUP"),
                            GROUP_CODE = newItp.GROUP_CODE,
                            ITEM = getCodeName(newItp.PART_CODE, newItp.GROUP_CODE, newItp.ITEM_CODE, "ITEM"),
                            ITEM_CODE = newItp.ITEM_CODE,
                            INSPECTION = newItp.INSPECTION,
                            ITP_STATUS = CommonSettingData.Dis_Itp_Status.NEW.ToString(),
                            REVISION_STATUS = _REVISION_STATUS,
                            ACTION_STATUS = CommonSettingData.Dis_Action_Status.REVISION.ToString(),
                            CLIENT_REFERENCE_NO = newItp.CLIENT_REFERENCE_NO,
                            REMARK = newItp.REMARK,
                            OUT_OF_USE = false,
                            REG_DATE = DateTime.Now,
                            CREATE_USER_NAME = HttpContext.Session.GetString("UserName"),
                            ITP_VERSION = _revisionNo
                        };

                        if (newItp.CODE != null)
                        {
                            // ITP Master에 등록되어 있는 File 정보를 확인해 TNAV_COM_FILE에 정보만 저장
                            // 1. CODE를 이용해 ITP Master ITP_IDX를 조회
                            Guid _itp_Idx = _repository.TNAV_DIS_ITP_MASTERs.Where(m => m.CODE == newItp.CODE).Select(m => m.ITP_IDX).FirstOrDefault();

                            // 2. ITP Master ITP_IDX를 이용해 COM_FILES 의 정보 조회
                            List<TNAV_COM_FILE> fileList = _repository.TNAV_COM_FILEs.Where(m => m.DOCUMENT_IDX.Equals(_itp_Idx)).AsNoTracking().ToList();

                            // 3. 신규 ITP에 조회한 ITP Master Com_File 정보를 복사
                            List<TNAV_COM_FILE> NewItpFiles = new List<TNAV_COM_FILE>();
                            Guid _idx = Guid.NewGuid();
                            foreach (TNAV_COM_FILE file in fileList)
                            {
                                file.IDX = Guid.NewGuid();
                                string DestinationFullPath = await Uploader.FileHandler.ChangeSavedFullPath(file.SAVED_FULL_PATH, _idx, "DIS", newItp.PROJECT_ID, newItp.WORK_ID);
                                string DestinationFileName = await Uploader.FileHandler.ChangeSavedFileName(DestinationFullPath);

                                file.DOCUMENT_IDX = _DIS_ITP_CONTROL.ITP_IDX;
                                file.KIND_OF_FILES = "ITP_CONTROL";
                                file.RELATED_INFO = _itp_Idx.ToString();
                                file.PROJECT_IDX = newItp.PROJECT_IDX;
                                file.PROJECT_ID = newItp.PROJECT_ID;
                                file.WORK_IDX = newItp.WORK_IDX;
                                file.WORK_ID = newItp.WORK_ID;
                                file.REG_DATE = DateTime.Now;
                                file.CREATE_USER_NAME = HttpContext.Session.GetString("UserName");
                                file.SAVED_FULL_PATH = DestinationFullPath;
                                file.SAVED_FILENAME = DestinationFileName;

                                NewItpFiles.Add(file);
                            }

                            await _repository.AddRangeAsync(NewItpFiles);
                        }
                        else
                        {
                            // 신규 첨부, Master ITP를 사용하지 않고 첨부한 경우
                            //UploaderController에서 첨부파일 추가
                        }

                        await _repository.AddAsync(_DIS_ITP_CONTROL);
                    }

                    await _repository.SaveChangesAsync();

                    return Json(new
                    {
                        result = "OK",
                        Work_IDX = newItp.WORK_IDX,
                        TempItpIdx = Guid.NewGuid().ToString().ToUpper()
                    });
                }
                else
                {
                    return RedirectToAction(nameof(Create), new { id = newItp.WORK_IDX });
                }
            }
            catch (Exception e)
            {
                return RedirectToAction("SaveException", "Error", new { ex = e.InnerException.Message, returnController = "DisItpControl", returnView = "Detail" });
            }
        }

        /// <summary>
        /// Revision ITP 수정
        /// </summary>
        /// <param name="newItp"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ModifyExistItp(TNAV_DIS_ITP_CONTROL newItp, Guid TempItpIdx)
        {
            try
            {
                if (ExistItp(newItp.ITP_IDX))
                {
                    // 기존 ITP 삭제 (Revision Status -> DELETE)
                    TNAV_DIS_ITP_CONTROL EXIST_DIS_ITP_CONTROL = await _repository.TNAV_DIS_ITP_CONTROLs.Where(m => m.ITP_IDX == newItp.ITP_IDX).FirstAsync();
                    EXIST_DIS_ITP_CONTROL.REVISION_STATUS = CommonSettingData.Dis_Revision_Status.DELETE.ToString();
                    EXIST_DIS_ITP_CONTROL.DELETED_DATE = DateTime.Now;
                    EXIST_DIS_ITP_CONTROL.DELETE_USER_NAME = HttpContext.Session.GetString("UserName");

                    // 수정하는 ITP 추가 (Revision Status -> ADD)
                    TNAV_DIS_ITP_CONTROL MODIFY_DIS_ITP_CONTROL = new TNAV_DIS_ITP_CONTROL()
                    {
                        ITP_IDX = TempItpIdx,
                        ITP_NO = newItp.ITP_NO,
                        PROJECT_IDX = newItp.PROJECT_IDX,
                        PROJECT_ID = newItp.PROJECT_ID,
                        WORK_IDX = newItp.WORK_IDX,
                        WORK_ID = newItp.WORK_ID,
                        CODE = String.Empty,
                        TYPE = String.Empty,
                        PART = getCodeName(newItp.PART_CODE, newItp.GROUP_CODE, newItp.ITEM_CODE, "PART"),
                        PART_CODE = newItp.PART_CODE,
                        GROUP = getCodeName(newItp.PART_CODE, newItp.GROUP_CODE, newItp.ITEM_CODE, "GROUP"),
                        GROUP_CODE = newItp.GROUP_CODE,
                        ITEM = getCodeName(newItp.PART_CODE, newItp.GROUP_CODE, newItp.ITEM_CODE, "ITEM"),
                        ITEM_CODE = newItp.ITEM_CODE,
                        INSPECTION = newItp.INSPECTION,
                        ITP_STATUS = CommonSettingData.Dis_Itp_Status.NEW.ToString(),
                        REVISION_STATUS = CommonSettingData.Dis_Revision_Status.ADD.ToString(),
                        ACTION_STATUS = CommonSettingData.Dis_Action_Status.REVISION.ToString(),
                        CLIENT_REFERENCE_NO = newItp.CLIENT_REFERENCE_NO,
                        REMARK = newItp.REMARK,
                        OUT_OF_USE = newItp.OUT_OF_USE,
                        REG_DATE = DateTime.Now,
                        CREATE_USER_NAME = HttpContext.Session.GetString("UserName"),
                        ITP_VERSION = newItp.ITP_VERSION
                    };


                    _repository.Update(EXIST_DIS_ITP_CONTROL);
                    _repository.Add(MODIFY_DIS_ITP_CONTROL);
                    await _repository.SaveChangesAsync();

                    return Json(new
                    {
                        result = "OK",
                        Work_ID = newItp.WORK_IDX,
                        TempItpIdx = Guid.NewGuid().ToString().ToUpper()
                    });
                }
                else
                {
                    //return View(newItp);
                    return RedirectToAction(nameof(Create), new { id = newItp.WORK_IDX });
                }
            }
            catch (Exception e)
            {
                return RedirectToAction("SaveException", "Error", new { ex = e.InnerException.Message, returnController = "DisItpControl", returnView = "Create" });
            }
        }

        /// <summary>
        /// Revision ITP 삭제
        /// </summary>
        /// <param name="newItp"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteExistItp(TNAV_DIS_ITP_CONTROL newItp)
        {
            try
            {
                if (newItp != null)
                {
                    if (ExistItp(newItp.ITP_IDX))
                    {
                        // ITP 삭제 (Revision Status -> DELETE)
                        TNAV_DIS_ITP_CONTROL EXIST_DIS_ITP_CONTROL = await _repository.TNAV_DIS_ITP_CONTROLs.Where(m => m.ITP_IDX == newItp.ITP_IDX).FirstAsync();
                        EXIST_DIS_ITP_CONTROL.REVISION_STATUS = CommonSettingData.Dis_Revision_Status.DELETE.ToString();
                        EXIST_DIS_ITP_CONTROL.DELETED_DATE = DateTime.Now;
                        EXIST_DIS_ITP_CONTROL.DELETE_USER_NAME = HttpContext.Session.GetString("UserName");

                        _repository.Update(EXIST_DIS_ITP_CONTROL);
                        await _repository.SaveChangesAsync();
                    }

                    return Json(new
                    {
                        result = "OK",
                        Work_ID = newItp.WORK_IDX
                    });
                }
                else
                {
                    return RedirectToAction(nameof(Create), new { id = newItp.WORK_IDX });
                }
            }
            catch (Exception e)
            {
                return RedirectToAction("SaveException", "Error", new { ex = e.InnerException.Message, returnController = "DisItpControl", returnView = "Create" });
            }
        }

        /// <summary>
        /// Revision ITP가 존재하는지 확인
        /// </summary>
        /// <param name="itpIdx"></param>
        /// <returns></returns>
        public bool ExistItp(Guid itpIdx)
        {
            return _repository.TNAV_DIS_ITP_CONTROLs.Where(m => m.ITP_IDX == itpIdx).Any();
        }

        /// <summary>
        /// 코드화된 내용
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public string getCodeName(string _part_code, string _group_code, string _item_code, string type)
        {
            string result = string.Empty;
            switch (type)
            {
                case "PART":
                    result = _repository.TNAV_DIS_ITP_MASTERs.Where(m => m.PART_CODE == _part_code).Select(m => m.PART).First();
                    break;
                case "GROUP":
                    result = _repository.TNAV_DIS_ITP_MASTERs.Where(m => m.PART_CODE == _part_code && m.GROUP_CODE == _group_code).Select(m => m.GROUP).First();
                    break;
                case "ITEM":
                    result = _repository.TNAV_DIS_ITP_MASTERs.Where(m => m.PART_CODE == _part_code && m.GROUP_CODE == _group_code && m.ITEM_CODE == _item_code).Select(m => m.ITEM).First();
                    break;
            }
            return result;
        }
    }
}
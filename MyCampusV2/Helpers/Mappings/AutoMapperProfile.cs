using AutoMapper;
using MyCampusV2.Models.V2.entity;
using System;
using MyCampusV2.Common.ViewModels;
using MyCampusV2.Models.Reports;
using MyCampusV2.Common.ViewModels.ReportViewModel;
using MyCampusV2.Models;
using MyCampusV2.Helpers.Encryption;
using System.Globalization;

namespace MyCampusV2.CustomHelpers.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            #region Digital References

            CreateMap<digitalReferencesVM, digitalReferencesEntity>()
                .ForMember(x => x.Digital_Reference_Code, opt => opt.MapFrom(src => src.digitalReferenceCode == null ? string.Empty : src.digitalReferenceCode))
                .ForMember(x => x.Title, opt => opt.MapFrom(src => src.title))
                .ForMember(x => x.File_Path, opt => opt.MapFrom(src => src.filePath == null ? string.Empty : src.filePath))
                .ForMember(x => x.File_Type, opt => opt.MapFrom(src => src.fileType == null ? string.Empty : src.fileType))
                .ForMember(x => x.File_Name, opt => opt.MapFrom(src => src.fileName))
                .ForMember(x => x.Date_Uploaded, opt => opt.MapFrom(src => src.dateUploaded))
                .ForMember(x => x.Added_By, opt => opt.MapFrom(src => src.addedBy))
                .ForMember(x => x.Updated_By, opt => opt.MapFrom(src => src.updatedBy))
                .ForMember(x => x.IsActive, opt => opt.MapFrom(src => src.isActive))
                .ForMember(x => x.ToDisplay, opt => opt.MapFrom(src => src.toDisplay))
                ;

            CreateMap<digitalReferencesEntity, digitalReferencesVM>()
                .ForMember(x => x.digitalReferenceCode, opt => opt.MapFrom(src => src.Digital_Reference_Code))
                .ForMember(x => x.title, opt => opt.MapFrom(src => src.Title))
                .ForMember(x => x.filePath, opt => opt.MapFrom(src => src.File_Path))
                .ForMember(x => x.fileType, opt => opt.MapFrom(src => src.File_Type))
                .ForMember(x => x.fileName, opt => opt.MapFrom(src => src.File_Name))
                .ForMember(x => x.addedBy, opt => opt.MapFrom(src => src.Added_By))
                .ForMember(x => x.updatedBy, opt => opt.MapFrom(src => src.Updated_By))
                .ForMember(x => x.isActive, opt => opt.MapFrom(src => src.IsActive))
                .ForMember(x => x.toDisplay, opt => opt.MapFrom(src => src.ToDisplay))
                .ForMember(x => x.dateUploaded, opt => opt.MapFrom(src => src.Date_Uploaded.ToString("yyyy/MM/dd")))
                ;

            CreateMap<digitalReferencesPagedResult, digitalReferencesPagedResultVM>();

            #endregion

            #region Calendar

            CreateMap<yearSectionEntity, calendarRecipientsVM>()
                .ForMember(x => x.key, opt => opt.MapFrom(src => src.YearSec_Name))
                .ForMember(x => x.value, opt => opt.MapFrom(src => src.YearSec_Name))
                ;

            CreateMap<calendarVM, calendarEntity>()
                .ForMember(x => x.Calendar_Code, opt => opt.MapFrom(src => src.calendarCode))
                .ForMember(x => x.Title, opt => opt.MapFrom(src => src.title))
                .ForMember(x => x.Body, opt => opt.MapFrom(src => src.body))
                .ForMember(x => x.IsAll, opt => opt.MapFrom(src => src.isAll))
                .ForMember(x => x.IsSent, opt => opt.MapFrom(src => src.isSent))
                .ForMember(x => x.Post_Date, opt => opt.MapFrom(src => Convert.ToDateTime(src.postDate)))
                .ForMember(x => x.Recipients, opt => opt.MapFrom(src => src.recipients))
                .ForMember(x => x.Added_By, opt => opt.MapFrom(src => src.addedBy))
                .ForMember(x => x.Updated_By, opt => opt.MapFrom(src => src.updatedBy))
                .ForMember(x => x.IsActive, opt => opt.MapFrom(src => src.isActive))
                .ForMember(x => x.ToDisplay, opt => opt.MapFrom(src => src.toDisplay))
                ;

            CreateMap<calendarEntity, calendarVM>()
                .ForMember(x => x.calendarCode, opt => opt.MapFrom(src => src.Calendar_Code))
                .ForMember(x => x.title, opt => opt.MapFrom(src => src.Title))
                .ForMember(x => x.body, opt => opt.MapFrom(src => src.Body))
                .ForMember(x => x.isAll, opt => opt.MapFrom(src => src.IsAll))
                .ForMember(x => x.isSent, opt => opt.MapFrom(src => src.IsSent))
                .ForMember(x => x.recipientsDisplay, opt => opt.MapFrom(src => src.Year_Level))
                .ForMember(x => x.recipients, opt => opt.MapFrom(src => src.Recipients))
                .ForMember(x => x.addedBy, opt => opt.MapFrom(src => src.Added_By))
                .ForMember(x => x.updatedBy, opt => opt.MapFrom(src => src.Updated_By))
                .ForMember(x => x.isActive, opt => opt.MapFrom(src => src.IsActive))
                .ForMember(x => x.toDisplay, opt => opt.MapFrom(src => src.ToDisplay))
                .ForMember(x => x.postDate, opt => opt.MapFrom(src => src.Post_Date.ToString("yyyy/MM/dd")))
                ;

            CreateMap<calendarPagedResult, calendarPagedResultVM>();

            #endregion

            #region Announcements

            CreateMap<yearSectionEntity, recipientsVM>()
                .ForMember(x => x.key, opt => opt.MapFrom(src => src.YearSec_Name))
                .ForMember(x => x.value, opt => opt.MapFrom(src => src.YearSec_Name))
                ;

            CreateMap<announcementsVM, announcementsEntity>()
                .ForMember(x => x.Announcement_Code, opt => opt.MapFrom(src => src.announcementsCode))
                .ForMember(x => x.Title, opt => opt.MapFrom(src => src.title))
                .ForMember(x => x.Body, opt => opt.MapFrom(src => src.body))
                .ForMember(x => x.IsAll, opt => opt.MapFrom(src => src.isAll))
                .ForMember(x => x.IsSent, opt => opt.MapFrom(src => src.isSent))
                //.ForMember(x => x.Date_Sent, opt => opt.MapFrom(src => src.dateSent))
                .ForMember(x => x.Recipients, opt => opt.MapFrom(src => src.recipients))
                .ForMember(x => x.Added_By, opt => opt.MapFrom(src => src.addedBy))
                .ForMember(x => x.Updated_By, opt => opt.MapFrom(src => src.updatedBy))
                .ForMember(x => x.IsActive, opt => opt.MapFrom(src => src.isActive))
                .ForMember(x => x.ToDisplay, opt => opt.MapFrom(src => src.toDisplay))
                ;

            CreateMap<announcementsEntity, announcementsVM>()
                .ForMember(x => x.announcementsCode, opt => opt.MapFrom(src => src.Announcement_Code))
                .ForMember(x => x.title, opt => opt.MapFrom(src => src.Title))
                .ForMember(x => x.body, opt => opt.MapFrom(src => src.Body))
                .ForMember(x => x.isAll, opt => opt.MapFrom(src => src.IsAll))
                .ForMember(x => x.isSent, opt => opt.MapFrom(src => src.IsSent))
                .ForMember(x => x.recipientsDisplay, opt => opt.MapFrom(src => src.Year_Level))
                .ForMember(x => x.recipients, opt => opt.MapFrom(src => src.Recipients))
                .ForMember(x => x.addedBy, opt => opt.MapFrom(src => src.Added_By))
                .ForMember(x => x.updatedBy, opt => opt.MapFrom(src => src.Updated_By))
                .ForMember(x => x.isActive, opt => opt.MapFrom(src => src.IsActive))
                .ForMember(x => x.toDisplay, opt => opt.MapFrom(src => src.ToDisplay))
                .ForMember(x => x.dateSent, opt => opt.MapFrom(src => src.Date_Sent.ToString("yyyy/MM/dd h:mm tt")))
                ;

            CreateMap<announcementsPagedResult, announcementsPagedResultVM>();

            #endregion

            #region PAPAccount

            CreateMap<personEntity, studentsVM>()
                .ForMember(x => x.key, opt => opt.MapFrom(src => src.ID_Number))
                .ForMember(x => x.value, opt => opt.MapFrom(src => (src.First_Name + " " + src.Last_Name + " (" + src.ID_Number + ")")))
                ;

            CreateMap<papAccountVM, papAccountEntity>()
                .ForMember(x => x.First_Name, opt => opt.MapFrom(src => src.firstName))
                .ForMember(x => x.Middle_Name, opt => opt.MapFrom(src => src.middleName == null ? string.Empty : src.middleName))
                .ForMember(x => x.Last_Name, opt => opt.MapFrom(src => src.lastName))
                .ForMember(x => x.Email_Address, opt => opt.MapFrom(src => src.emailAddress))
                .ForMember(x => x.Mobile_Number, opt => opt.MapFrom(src => src.mobileNumber))
                .ForMember(x => x.Linked_Students, opt => opt.MapFrom(src => src.linkedStudents))
                .ForMember(x => x.Account_Code, opt => opt.MapFrom(src => src.accountCode))
                .ForMember(x => x.Added_By, opt => opt.MapFrom(src => src.addedBy))
                .ForMember(x => x.Updated_By, opt => opt.MapFrom(src => src.updatedBy))
                .ForMember(x => x.IsActive, opt => opt.MapFrom(src => src.isActive))
                .ForMember(x => x.ToDisplay, opt => opt.MapFrom(src => src.toDisplay))
                ;

            CreateMap<papAccountEntity, papAccountVM>()
                .ForMember(x => x.firstName, opt => opt.MapFrom(src => src.First_Name))
                .ForMember(x => x.middleName, opt => opt.MapFrom(src => src.Middle_Name == null ? string.Empty : src.Middle_Name))
                .ForMember(x => x.lastName, opt => opt.MapFrom(src => src.Last_Name))
                .ForMember(x => x.emailAddress, opt => opt.MapFrom(src => src.Email_Address))
                .ForMember(x => x.mobileNumber, opt => opt.MapFrom(src => src.Mobile_Number))
                .ForMember(x => x.accountCode, opt => opt.MapFrom(src => src.Account_Code))
                .ForMember(x => x.linkedStudents, opt => opt.MapFrom(src => src.Linked_Students))
                .ForMember(x => x.lnkStudents, opt => opt.MapFrom(src => src.Lkd_Students))
                .ForMember(x => x.addedBy, opt => opt.MapFrom(src => src.Added_By))
                .ForMember(x => x.updatedBy, opt => opt.MapFrom(src => src.Updated_By))
                .ForMember(x => x.isActive, opt => opt.MapFrom(src => src.IsActive))
                .ForMember(x => x.toDisplay, opt => opt.MapFrom(src => src.ToDisplay))
                .ForMember(x => x.isPending, opt => opt.MapFrom(src => src.IsPending))
                .ForMember(x => x.dateAdded, opt => opt.MapFrom(src => src.Date_Time_Added.ToString("yyyy/MM/dd h:mm tt")))
                .ForMember(x => x.fullName, opt => opt.MapFrom(src => (src.First_Name + " " + (src.Middle_Name == null || src.Middle_Name == "" ? string.Empty : src.Middle_Name.Substring(0,1) + ".") + " " + src.Last_Name)))
                ;

            CreateMap<papAccountPagedResult, papAccountPagedResultVM>()
                .ForMember(x => x.CurrentPage, opt => opt.MapFrom(src => src.CurrentPage))
                .ForMember(x => x.PageCount, opt => opt.MapFrom(src => src.PageCount))
                .ForMember(x => x.PageSize, opt => opt.MapFrom(src => src.PageSize))
                .ForMember(x => x.RowCount, opt => opt.MapFrom(src => src.RowCount))
                .ForMember(x => x.FirstRowOnPage, opt => opt.MapFrom(src => src.FirstRowOnPage))
                .ForMember(x => x.LastRowOnPage, opt => opt.MapFrom(src => src.LastRowOnPage))
                .ForMember(x => x.papAccounts, opt => opt.MapFrom(src => src.papAccounts))
                ;

            CreateMap<papAccountLinkedStudentsEntity, papAccountLinkedStudentsVM>()
                .ForMember(x => x.firstName, opt => opt.MapFrom(src => src.PersonEntity.First_Name))
                .ForMember(x => x.middleName, opt => opt.MapFrom(src => src.PersonEntity.Middle_Name == null ? string.Empty : src.PersonEntity.Middle_Name))
                .ForMember(x => x.lastName, opt => opt.MapFrom(src => src.PersonEntity.Last_Name))
                .ForMember(x => x.idNumber, opt => opt.MapFrom(src => src.PersonEntity.ID_Number))
                .ForMember(x => x.yearLevelName, opt => opt.MapFrom(src => src.PersonEntity.StudentSectionEntity.YearSectionEntity.YearSec_Name))
                .ForMember(x => x.sectionName, opt => opt.MapFrom(src => src.PersonEntity.StudentSectionEntity.Description))
                ;

            #endregion

            #region CampusMapping

            CreateMap<campusEntity, campusVM>()
                 .ForMember(x => x.campusId, opt => opt.MapFrom(src => src.Campus_ID))
                 .ForMember(x => x.campusName, opt => opt.MapFrom(src => src.Campus_Name))
                 .ForMember(x => x.campusCode, opt => opt.MapFrom(src => src.Campus_Code))
                 .ForMember(x => x.campusStatus, opt => opt.MapFrom(src => src.Campus_Status))
                 .ForMember(x => x.campusAddress, opt => opt.MapFrom(src => src.Campus_Address))
                 .ForMember(x => x.campusContactNo, opt => opt.MapFrom(src => src.Campus_ContactNo))
                 .ForMember(x => x.divisionId, opt => opt.MapFrom(src => src.Division_ID))
                 .ForMember(x => x.divisionName, opt => opt.MapFrom(src => src.DivisionEntity.Name))
                 .ForMember(x => x.regionId, opt => opt.MapFrom(src => src.DivisionEntity.Region_ID))
                 .ForMember(x => x.regionName, opt => opt.MapFrom(src => src.DivisionEntity.RegionEntity.Name))
                 .ForMember(x => x.addedBy, opt => opt.MapFrom(src => src.Added_By))
                 .ForMember(x => x.updatedBy, opt => opt.MapFrom(src => src.Updated_By))
                 .ForMember(x => x.isActive, opt => opt.MapFrom(src => src.IsActive))
                 .ForMember(x => x.toDisplay, opt => opt.MapFrom(src => src.ToDisplay));

            CreateMap<campusVM, campusEntity>()
                .ForMember(x => x.Campus_ID, opt => opt.MapFrom(src => src.campusId))
                .ForMember(x => x.Campus_Code, opt => opt.MapFrom(src => src.campusCode))
                .ForMember(x => x.Campus_Name, opt => opt.MapFrom(src => src.campusName))
                .ForMember(x => x.Campus_Status, opt => opt.MapFrom(src => src.campusStatus))
                .ForMember(x => x.Campus_Address, opt => opt.MapFrom(src => src.campusAddress))
                .ForMember(x => x.Campus_ContactNo, opt => opt.MapFrom(src => src.campusContactNo))
                .ForMember(x => x.Division_ID, opt => opt.MapFrom(src => src.divisionId))
                .ForMember(x => x.Added_By, opt => opt.MapFrom(src => src.addedBy))
                .ForMember(x => x.Updated_By, opt => opt.MapFrom(src => src.updatedBy))
                .ForMember(x => x.IsActive, opt => opt.MapFrom(src => src.isActive))
                .ForMember(x => x.ToDisplay, opt => opt.MapFrom(src => src.toDisplay));

            CreateMap<campusBatchUploadHeaders, campusBatchUploadVM>()
              .ForMember(x => x.CampusName, opt => opt.MapFrom(src => src.CampusName));

            CreateMap<campusPagedResult, campusPagedResultVM>();

            #endregion

            #region AreaMapping

            CreateMap<areaEntity, areaVM>()
                .ForMember(x => x.areaId, opt => opt.MapFrom(src => src.Area_ID))
                .ForMember(x => x.areaName, opt => opt.MapFrom(src => src.Area_Name))
                .ForMember(x => x.areaDescription, opt => opt.MapFrom(src => src.Area_Description))
                .ForMember(x => x.campusId, opt => opt.MapFrom(src => src.Campus_ID))
                .ForMember(x => x.campusName, opt => opt.MapFrom(src => src.CampusEntity.Campus_Name))
                .ForMember(x => x.addedBy, opt => opt.MapFrom(src => src.Added_By))
                .ForMember(x => x.updatedBy, opt => opt.MapFrom(src => src.Updated_By))
                .ForMember(x => x.isActive, opt => opt.MapFrom(src => src.IsActive))
                .ForMember(x => x.toDisplay, opt => opt.MapFrom(src => src.ToDisplay));

            CreateMap<areaVM, areaEntity>()
                .ForMember(x => x.Area_ID, opt => opt.MapFrom(src => src.areaId))
                .ForMember(x => x.Area_Name, opt => opt.MapFrom(src => src.areaName))
                .ForMember(x => x.Area_Description, opt => opt.MapFrom(src => src.areaDescription))
                .ForMember(x => x.Campus_ID, opt => opt.MapFrom(src => src.campusId))
                .ForMember(x => x.Added_By, opt => opt.MapFrom(src => src.addedBy))
                .ForMember(x => x.Updated_By, opt => opt.MapFrom(src => src.updatedBy))
                .ForMember(x => x.IsActive, opt => opt.MapFrom(src => src.isActive))
                .ForMember(x => x.ToDisplay, opt => opt.MapFrom(src => src.toDisplay));

            CreateMap<areaBatchUploadHeaders, areaBatchUploadVM>()
                .ForMember(x => x.CampusName, opt => opt.MapFrom(src => src.CampusName))
                .ForMember(x => x.AreaName, opt => opt.MapFrom(src => src.AreaName))
                .ForMember(x => x.AreaDescription, opt => opt.MapFrom(src => src.AreaDescription));

            CreateMap<areaPagedResult, areaPagedResultVM>();

            #endregion

            #region EducLevelMapping

            CreateMap<educationalLevelEntity, educlevelVM>()
                .ForMember(x => x.educLevelId, opt => opt.MapFrom(src => src.Level_ID))
                .ForMember(x => x.educLevelName, opt => opt.MapFrom(src => src.Level_Name))
                .ForMember(x => x.educLevelStatus, opt => opt.MapFrom(src => src.Level_Status))
                .ForMember(x => x.hasCourse, opt => opt.MapFrom(src => src.hasCourse))
                .ForMember(x => x.campusId, opt => opt.MapFrom(src => src.Campus_ID))
                .ForMember(x => x.campusName, opt => opt.MapFrom(src => src.CampusEntity.Campus_Name))
                .ForMember(x => x.addedBy, opt => opt.MapFrom(src => src.Added_By))
                .ForMember(x => x.updatedBy, opt => opt.MapFrom(src => src.Updated_By))
                .ForMember(x => x.isActive, opt => opt.MapFrom(src => src.IsActive))
                .ForMember(x => x.toDisplay, opt => opt.MapFrom(src => src.ToDisplay));

            CreateMap<educlevelVM, educationalLevelEntity>()
                .ForMember(x => x.Level_ID, opt => opt.MapFrom(src => src.educLevelId))
                .ForMember(x => x.Level_Name, opt => opt.MapFrom(src => src.educLevelName))
                .ForMember(x => x.Level_Status, opt => opt.MapFrom(src => src.educLevelStatus))
                .ForMember(x => x.hasCourse, opt => opt.MapFrom(src => src.hasCourse))
                .ForMember(x => x.Campus_ID, opt => opt.MapFrom(src => src.campusId))
                .ForMember(x => x.Added_By, opt => opt.MapFrom(src => src.addedBy))
                .ForMember(x => x.Updated_By, opt => opt.MapFrom(src => src.updatedBy))
                .ForMember(x => x.IsActive, opt => opt.MapFrom(src => src.isActive))
                .ForMember(x => x.ToDisplay, opt => opt.MapFrom(src => src.toDisplay));

            CreateMap<educLevelBatchUploadHeaders, educLevelBatchUploadVM>()
                .ForMember(x => x.CampusName, opt => opt.MapFrom(src => src.CampusName))
                .ForMember(x => x.CampusName, opt => opt.MapFrom(src => src.EducationalLevelName))
                .ForMember(x => x.EducationalLevelStatus, opt => opt.MapFrom(src => src.EducationalLevelStatus))
                .ForMember(x => x.College, opt => opt.MapFrom(src => (src.College == "Yes" ? "Yes" : "No")));

            CreateMap<educlevelPagedResult, educlevelPagedResultVM>();

            #endregion

            #region YearSectionMapping

            CreateMap<yearSectionEntity, yearSectionVM>()
                .ForMember(x => x.yearSecId, opt => opt.MapFrom(src => src.YearSec_ID))
                .ForMember(x => x.yearSecName, opt => opt.MapFrom(src => src.YearSec_Name))
                .ForMember(x => x.educLevelId, opt => opt.MapFrom(src => src.Level_ID))
                .ForMember(x => x.educLevelName, opt => opt.MapFrom(src => src.EducationalLevelEntity.Level_Name))
                .ForMember(x => x.campusId, opt => opt.MapFrom(src => src.EducationalLevelEntity.Campus_ID))
                .ForMember(x => x.campusName, opt => opt.MapFrom(src => src.EducationalLevelEntity.CampusEntity.Campus_Name))
                .ForMember(x => x.addedBy, opt => opt.MapFrom(src => src.Added_By))
                .ForMember(x => x.updatedBy, opt => opt.MapFrom(src => src.Updated_By))
                .ForMember(x => x.isActive, opt => opt.MapFrom(src => src.IsActive))
                .ForMember(x => x.toDisplay, opt => opt.MapFrom(src => src.ToDisplay));

            CreateMap<yearSectionVM, yearSectionEntity>()
                .ForMember(x => x.YearSec_ID, opt => opt.MapFrom(src => src.yearSecId))
                .ForMember(x => x.YearSec_Name, opt => opt.MapFrom(src => src.yearSecName))
                .ForMember(x => x.Level_ID, opt => opt.MapFrom(src => src.educLevelId))
                .ForMember(x => x.Added_By, opt => opt.MapFrom(src => src.addedBy))
                .ForMember(x => x.Updated_By, opt => opt.MapFrom(src => src.updatedBy))
                .ForMember(x => x.IsActive, opt => opt.MapFrom(src => src.isActive))
                .ForMember(x => x.ToDisplay, opt => opt.MapFrom(src => src.toDisplay));

            CreateMap<yearSectionPagedResult, yearSectionPagedResultVM>();

            #endregion

            #region StudentSectionMapping

            CreateMap<studentSectionEntity, studentSectionVM>()
                .ForMember(x => x.studSecId, opt => opt.MapFrom(src => src.StudSec_ID))
                .ForMember(x => x.description, opt => opt.MapFrom(src => src.Description))
                .ForMember(x => x.yearSecId, opt => opt.MapFrom(src => src.YearSec_ID))
                .ForMember(x => x.yearSecName, opt => opt.MapFrom(src => src.YearSectionEntity.YearSec_Name))
                .ForMember(x => x.educLevelId, opt => opt.MapFrom(src => src.YearSectionEntity.Level_ID))
                .ForMember(x => x.educLevelName, opt => opt.MapFrom(src => src.YearSectionEntity.EducationalLevelEntity.Level_Name))
                .ForMember(x => x.campusId, opt => opt.MapFrom(src => src.YearSectionEntity.EducationalLevelEntity.Campus_ID))
                .ForMember(x => x.campusName, opt => opt.MapFrom(src => src.YearSectionEntity.EducationalLevelEntity.CampusEntity.Campus_Name))
                .ForMember(x => x.startTime, opt => opt.MapFrom(src => src.Start_Time))
                .ForMember(x => x.endTime, opt => opt.MapFrom(src => src.End_Time))
                .ForMember(x => x.halfDay, opt => opt.MapFrom(src => src.Half_Day))
                .ForMember(x => x.gracePeriod, opt => opt.MapFrom(src => src.Grace_Period))
                .ForMember(x => x.password, opt => opt.MapFrom(src => src.Password))
                .ForMember(x => x.addedBy, opt => opt.MapFrom(src => src.Added_By))
                .ForMember(x => x.updatedBy, opt => opt.MapFrom(src => src.Updated_By))
                .ForMember(x => x.isActive, opt => opt.MapFrom(src => src.IsActive))
                .ForMember(x => x.toDisplay, opt => opt.MapFrom(src => src.ToDisplay));

            CreateMap<studentSectionVM, studentSectionEntity>()
                .ForMember(x => x.StudSec_ID, opt => opt.MapFrom(src => src.studSecId))
                .ForMember(x => x.Description, opt => opt.MapFrom(src => src.description))
                .ForMember(x => x.YearSec_ID, opt => opt.MapFrom(src => src.yearSecId))
                .ForMember(x => x.Start_Time, opt => opt.MapFrom(src => src.startTime))
                .ForMember(x => x.End_Time, opt => opt.MapFrom(src => src.endTime))
                .ForMember(x => x.Half_Day, opt => opt.MapFrom(src => src.halfDay))
                .ForMember(x => x.Grace_Period, opt => opt.MapFrom(src => src.gracePeriod))
                .ForMember(x => x.Password, opt => opt.MapFrom(src => src.password))
                .ForMember(x => x.Added_By, opt => opt.MapFrom(src => src.addedBy))
                .ForMember(x => x.Updated_By, opt => opt.MapFrom(src => src.updatedBy))
                .ForMember(x => x.IsActive, opt => opt.MapFrom(src => src.isActive))
                .ForMember(x => x.ToDisplay, opt => opt.MapFrom(src => src.toDisplay));

            CreateMap<studentSecPagedResult, studentSecPagedResultVM>();

            CreateMap<sectionSchedulePagedResult, sectionSchedulePagedResultVM>();

            CreateMap<sectionScheduleEntity, sectionScheduleVM>()
                .ForMember(x => x.schedId, opt => opt.MapFrom(src => src.ID))
                .ForMember(x => x.schedStudSecId, opt => opt.MapFrom(src => src.StudSec_ID))
                .ForMember(x => x.schedDate, opt => opt.MapFrom(src => src.Schedule_Date.ToString("yyyy-MM-dd")))
                .ForMember(x => x.schedStartDate, opt => opt.MapFrom(src => src.Schedule_Start_Date.ToString("yyyy-MM-dd")))
                .ForMember(x => x.schedEndDate, opt => opt.MapFrom(src => src.Schedule_End_Date.ToString("yyyy-MM-dd")))
                .ForMember(x => x.schedStartTime, opt => opt.MapFrom(src => src.Start_Time.ToString(@"hh\:mm")))
                .ForMember(x => x.schedEndTime, opt => opt.MapFrom(src => src.End_Time.ToString(@"hh\:mm")))
                .ForMember(x => x.schedHalfDay, opt => opt.MapFrom(src => src.Half_Day.ToString(@"hh\:mm")))
                .ForMember(x => x.schedGracePeriod, opt => opt.MapFrom(src => src.Grace_Period))
                .ForMember(x => x.schedIsExcused, opt => opt.MapFrom(src => src.IsExcused))
                .ForMember(x => x.schedRemarks, opt => opt.MapFrom(src => src.Remarks))
                .ForMember(x => x.addedBy, opt => opt.MapFrom(src => src.Added_By))
                .ForMember(x => x.updatedBy, opt => opt.MapFrom(src => src.Updated_By));

            CreateMap<sectionScheduleVM, sectionScheduleEntity>()
                .ForMember(x => x.ID, opt => opt.MapFrom(src => src.schedId))
                .ForMember(x => x.StudSec_ID, opt => opt.MapFrom(src => src.schedStudSecId))
                .ForMember(x => x.Schedule_Date, opt => opt.MapFrom(src => src.schedDate))
                .ForMember(x => x.Schedule_Start_Date, opt => opt.MapFrom(src => src.schedStartDate))
                .ForMember(x => x.Schedule_End_Date, opt => opt.MapFrom(src => src.schedEndDate))
                .ForMember(x => x.Start_Time, opt => opt.MapFrom(src => src.schedStartTime))
                .ForMember(x => x.End_Time, opt => opt.MapFrom(src => src.schedEndTime))
                .ForMember(x => x.Half_Day, opt => opt.MapFrom(src => src.schedHalfDay))
                .ForMember(x => x.Grace_Period, opt => opt.MapFrom(src => src.schedGracePeriod))
                .ForMember(x => x.IsExcused, opt => opt.MapFrom(src => src.schedIsExcused))
                .ForMember(x => x.Remarks, opt => opt.MapFrom(src => src.schedRemarks))
                .ForMember(x => x.Added_By, opt => opt.MapFrom(src => src.addedBy))
                .ForMember(x => x.Updated_By, opt => opt.MapFrom(src => src.updatedBy));

            #endregion

            #region CollegeMapping

            CreateMap<collegeEntity, collegeVM>()
                .ForMember(x => x.collegeId, opt => opt.MapFrom(src => src.College_ID))
                .ForMember(x => x.collegeName, opt => opt.MapFrom(src => src.College_Name))
                .ForMember(x => x.educLevelId, opt => opt.MapFrom(src => src.Level_ID))
                .ForMember(x => x.educLevelName, opt => opt.MapFrom(src => src.EducationalLevelEntity.Level_Name))
                .ForMember(x => x.campusId, opt => opt.MapFrom(src => src.EducationalLevelEntity.Campus_ID))
                //.ForMember(x => x.campusId, opt => opt.MapFrom(src => src.Campus_ID))
                .ForMember(x => x.campusName, opt => opt.MapFrom(src => src.EducationalLevelEntity.CampusEntity.Campus_Name))
                .ForMember(x => x.addedBy, opt => opt.MapFrom(src => src.Added_By))
                .ForMember(x => x.updatedBy, opt => opt.MapFrom(src => src.Updated_By))
                .ForMember(x => x.isActive, opt => opt.MapFrom(src => src.IsActive))
                .ForMember(x => x.toDisplay, opt => opt.MapFrom(src => src.ToDisplay));

            CreateMap<collegeVM, collegeEntity>()
                .ForMember(x => x.College_ID, opt => opt.MapFrom(src => src.collegeId))
                .ForMember(x => x.College_Name, opt => opt.MapFrom(src => src.collegeName))
                .ForMember(x => x.Level_ID, opt => opt.MapFrom(src => src.educLevelId))
                .ForMember(x => x.Added_By, opt => opt.MapFrom(src => src.addedBy))
                .ForMember(x => x.Updated_By, opt => opt.MapFrom(src => src.updatedBy))
                .ForMember(x => x.IsActive, opt => opt.MapFrom(src => src.isActive))
                .ForMember(x => x.ToDisplay, opt => opt.MapFrom(src => src.toDisplay));

            CreateMap<collegeBatchUploadHeaders, collegeBatchUploadVM>()
                .ForMember(x => x.CampusName, opt => opt.MapFrom(src => src.CampusName))
                .ForMember(x => x.EducationalLevelName, opt => opt.MapFrom(src => src.EducationalLevelName))
                .ForMember(x => x.CollegeName, opt => opt.MapFrom(src => src.CollegeName))
                .ForMember(x => x.CollegeDescription, opt => opt.MapFrom(src => src.CollegeDescription));

            CreateMap<collegePagedResult, collegePagedResultVM>();

            #endregion

            #region CourseMapping

            CreateMap<courseEntity, courseVM>()
                .ForMember(x => x.courseId, opt => opt.MapFrom(src => src.Course_ID))
                .ForMember(x => x.courseName, opt => opt.MapFrom(src => src.Course_Name))
                .ForMember(x => x.collegeId, opt => opt.MapFrom(src => src.College_ID))
                .ForMember(x => x.collegeName, opt => opt.MapFrom(src => src.CollegeEntity.College_Name))
                .ForMember(x => x.educLevelId, opt => opt.MapFrom(src => src.CollegeEntity.Level_ID))
                .ForMember(x => x.educLevelName, opt => opt.MapFrom(src => src.CollegeEntity.EducationalLevelEntity.Level_Name))
                .ForMember(x => x.campusId, opt => opt.MapFrom(src => src.CollegeEntity.EducationalLevelEntity.Campus_ID))
                .ForMember(x => x.campusName, opt => opt.MapFrom(src => src.CollegeEntity.EducationalLevelEntity.CampusEntity.Campus_Name))
                .ForMember(x => x.addedBy, opt => opt.MapFrom(src => src.Added_By))
                .ForMember(x => x.updatedBy, opt => opt.MapFrom(src => src.Updated_By))
                .ForMember(x => x.isActive, opt => opt.MapFrom(src => src.IsActive))
                .ForMember(x => x.toDisplay, opt => opt.MapFrom(src => src.ToDisplay))
                .ForMember(x => x.lastUpdated, opt => opt.MapFrom(src => src.Last_Updated));

            CreateMap<courseVM, courseEntity>()
                .ForMember(x => x.Course_ID, opt => opt.MapFrom(src => src.courseId))
                .ForMember(x => x.Course_Name, opt => opt.MapFrom(src => src.courseName))
                .ForMember(x => x.College_ID, opt => opt.MapFrom(src => src.collegeId))
                .ForMember(x => x.Added_By, opt => opt.MapFrom(src => src.addedBy))
                .ForMember(x => x.Updated_By, opt => opt.MapFrom(src => src.updatedBy))
                .ForMember(x => x.IsActive, opt => opt.MapFrom(src => src.isActive))
                .ForMember(x => x.ToDisplay, opt => opt.MapFrom(src => src.toDisplay));

            CreateMap<courseBatchUploadHeaders, courseBatchUploadVM>()
                .ForMember(x => x.CampusName, opt => opt.MapFrom(src => src.CampusName))
                .ForMember(x => x.EducationalLevelName, opt => opt.MapFrom(src => src.EducationalLevelName))
                .ForMember(x => x.CollegeName, opt => opt.MapFrom(src => src.CollegeName))
                //.ForMember(x => x.c, opt => opt.MapFrom(src => src.CourseCode))
                .ForMember(x => x.CourseName, opt => opt.MapFrom(src => src.CourseName))
                .ForMember(x => x.CourseDescription, opt => opt.MapFrom(src => src.CourseDescription));

            CreateMap<coursePagedResult, coursePagedResultVM>();

            #endregion

            #region DepartmentMapping

            CreateMap<departmentEntity, departmentVM>()
                .ForMember(x => x.departmentId, opt => opt.MapFrom(src => src.Department_ID))
                .ForMember(x => x.departmentName, opt => opt.MapFrom(src => src.Department_Name))
                .ForMember(x => x.campusId, opt => opt.MapFrom(src => src.CampusEntity.Campus_ID))
                .ForMember(x => x.campusName, opt => opt.MapFrom(src => src.CampusEntity.Campus_Name))
                .ForMember(x => x.addedBy, opt => opt.MapFrom(src => src.Added_By))
                .ForMember(x => x.updatedBy, opt => opt.MapFrom(src => src.Updated_By))
                .ForMember(x => x.isActive, opt => opt.MapFrom(src => src.IsActive))
                .ForMember(x => x.toDisplay, opt => opt.MapFrom(src => src.ToDisplay));

            CreateMap<departmentVM, departmentEntity>()
                .ForMember(x => x.Department_ID, opt => opt.MapFrom(src => src.departmentId))
                .ForMember(x => x.Department_Name, opt => opt.MapFrom(src => src.departmentName))
                .ForMember(x => x.Campus_ID, opt => opt.MapFrom(src => src.campusId))
                .ForMember(x => x.Added_By, opt => opt.MapFrom(src => src.addedBy))
                .ForMember(x => x.Updated_By, opt => opt.MapFrom(src => src.updatedBy))
                .ForMember(x => x.IsActive, opt => opt.MapFrom(src => src.isActive))
                .ForMember(x => x.ToDisplay, opt => opt.MapFrom(src => src.toDisplay));

            CreateMap<departmentBatchUploadHeaders, departmentBatchUploadVM>()
                .ForMember(x => x.CampusName, opt => opt.MapFrom(src => src.CampusName))
                .ForMember(x => x.DepartmentName, opt => opt.MapFrom(src => src.DepartmentName));

            CreateMap<departmentPagedResult, departmentPagedResultVM>();

            #endregion

            #region PositionMapping

            CreateMap<positionEntity, positionVM>()
                .ForMember(x => x.positionId, opt => opt.MapFrom(src => src.Position_ID))
                .ForMember(x => x.positionName, opt => opt.MapFrom(src => src.Position_Name))
                .ForMember(x => x.departmentId, opt => opt.MapFrom(src => src.Department_ID))
                .ForMember(x => x.departmentName, opt => opt.MapFrom(src => src.DepartmentEntity.Department_Name))
                .ForMember(x => x.campusId, opt => opt.MapFrom(src => src.DepartmentEntity.Campus_ID))
                .ForMember(x => x.campusName, opt => opt.MapFrom(src => src.DepartmentEntity.CampusEntity.Campus_Name))
                .ForMember(x => x.addedBy, opt => opt.MapFrom(src => src.Added_By))
                .ForMember(x => x.updatedBy, opt => opt.MapFrom(src => src.Updated_By))
                .ForMember(x => x.isActive, opt => opt.MapFrom(src => src.IsActive))
                .ForMember(x => x.toDisplay, opt => opt.MapFrom(src => src.ToDisplay));

            CreateMap<positionVM, positionEntity>()
                .ForMember(x => x.Position_ID, opt => opt.MapFrom(src => src.positionId))
                .ForMember(x => x.Position_Name, opt => opt.MapFrom(src => src.positionName))
                .ForMember(x => x.Department_ID, opt => opt.MapFrom(src => src.departmentId))
                .ForMember(x => x.Added_By, opt => opt.MapFrom(src => src.addedBy))
                .ForMember(x => x.Updated_By, opt => opt.MapFrom(src => src.updatedBy))
                .ForMember(x => x.IsActive, opt => opt.MapFrom(src => src.isActive))
                .ForMember(x => x.ToDisplay, opt => opt.MapFrom(src => src.toDisplay));

            CreateMap<positionBatchUploadHeaders, positionBatchUploadVM>()
                .ForMember(x => x.CampusName, opt => opt.MapFrom(src => src.CampusName))
                .ForMember(x => x.PositionName, opt => opt.MapFrom(src => src.PositionName))
                .ForMember(x => x.PositionDescription, opt => opt.MapFrom(src => src.PositionDescription));

            CreateMap<positionPagedResult, positionPagedResultVM>();

            #endregion

            #region EmployeeType

            CreateMap<employeeTypeVM, empTypeEntity>()
                .ForMember(x => x.EmpType_ID, opt => opt.MapFrom(src => src.employeeTypeId))
                .ForMember(x => x.EmpTypeDesc, opt => opt.MapFrom(src => src.employeeTypeDesc))
                .ForMember(x => x.Campus_ID, opt => opt.MapFrom(src => src.campusId))
                .ForMember(x => x.Added_By, opt => opt.MapFrom(src => src.addedBy))
                .ForMember(x => x.Updated_By, opt => opt.MapFrom(src => src.updatedBy))
                .ForMember(x => x.IsActive, opt => opt.MapFrom(src => src.isActive))
                .ForMember(x => x.ToDisplay, opt => opt.MapFrom(src => src.toDisplay));

            CreateMap<empTypeEntity, employeeTypeVM>()
                .ForMember(x => x.employeeTypeId, opt => opt.MapFrom(src => src.EmpType_ID))
                .ForMember(x => x.employeeTypeDesc, opt => opt.MapFrom(src => src.EmpTypeDesc))
                .ForMember(x => x.campusId, opt => opt.MapFrom(src => src.Campus_ID))
                .ForMember(x => x.campusName, opt => opt.MapFrom(src => src.CampusEntity.Campus_Name))
                .ForMember(x => x.addedBy, opt => opt.MapFrom(src => src.Added_By))
                .ForMember(x => x.updatedBy, opt => opt.MapFrom(src => src.Updated_By))
                .ForMember(x => x.isActive, opt => opt.MapFrom(src => src.IsActive))
                .ForMember(x => x.toDisplay, opt => opt.MapFrom(src => src.ToDisplay));

            CreateMap<employeeTypePagedResult, employeeTypePagedResult>();

            #endregion

            #region EmployeeSubType

            CreateMap<employeeSubTypeVM, employeeSubTypeEntity>()
                .ForMember(x => x.EmpSubtype_ID, opt => opt.MapFrom(src => src.employeeSubTypeId))
                .ForMember(x => x.EmpSubTypeDesc, opt => opt.MapFrom(src => src.employeeSubTypeDesc))
                .ForMember(x => x.EmpType_ID, opt => opt.MapFrom(src => src.employeeTypeId))
                .ForMember(x => x.Added_By, opt => opt.MapFrom(src => src.addedBy))
                .ForMember(x => x.Updated_By, opt => opt.MapFrom(src => src.updatedBy))
                .ForMember(x => x.IsActive, opt => opt.MapFrom(src => src.isActive))
                .ForMember(x => x.ToDisplay, opt => opt.MapFrom(src => src.toDisplay));

            CreateMap<employeeSubTypeEntity, employeeSubTypeVM>()
                .ForMember(x => x.employeeSubTypeId, opt => opt.MapFrom(src => src.EmpSubtype_ID))
                .ForMember(x => x.employeeSubTypeDesc, opt => opt.MapFrom(src => src.EmpSubTypeDesc))
                .ForMember(x => x.employeeTypeId, opt => opt.MapFrom(src => src.EmpType_ID))
                .ForMember(x => x.employeeTypeDesc, opt => opt.MapFrom(src => src.EmployeeType.EmpTypeDesc))
                .ForMember(x => x.campusId, opt => opt.MapFrom(src => src.EmployeeType.Campus_ID))
                .ForMember(x => x.campusName, opt => opt.MapFrom(src => src.EmployeeType.CampusEntity.Campus_Name))
                .ForMember(x => x.addedBy, opt => opt.MapFrom(src => src.Added_By))
                .ForMember(x => x.updatedBy, opt => opt.MapFrom(src => src.Updated_By))
                .ForMember(x => x.isActive, opt => opt.MapFrom(src => src.IsActive))
                .ForMember(x => x.toDisplay, opt => opt.MapFrom(src => src.ToDisplay));

            CreateMap<employeeSubTypePagedResult, employeeSubTypePagedResultVM>();

            #endregion

            #region OfficeMapping

            CreateMap<officeVM, officeEntity>()
                .ForMember(x => x.Office_ID, opt => opt.MapFrom(src => src.officeId))
                .ForMember(x => x.Office_Name, opt => opt.MapFrom(src => src.officeName))
                .ForMember(x => x.Office_Status, opt => opt.MapFrom(src => src.officeStatus))
                .ForMember(x => x.Campus_ID, opt => opt.MapFrom(src => src.campusId))
                .ForMember(x => x.Added_By, opt => opt.MapFrom(src => src.addedBy))
                .ForMember(x => x.Updated_By, opt => opt.MapFrom(src => src.updatedBy))
                .ForMember(x => x.IsActive, opt => opt.MapFrom(src => src.isActive))
                .ForMember(x => x.ToDisplay, opt => opt.MapFrom(src => src.toDisplay));

            CreateMap<officeEntity, officeVM>()
                .ForMember(x => x.officeId, opt => opt.MapFrom(src => src.Office_ID))
                .ForMember(x => x.officeName, opt => opt.MapFrom(src => src.Office_Name))
                .ForMember(x => x.officeStatus, opt => opt.MapFrom(src => src.Office_Status))
                .ForMember(x => x.campusId, opt => opt.MapFrom(src => src.Campus_ID))
                .ForMember(x => x.campusName, opt => opt.MapFrom(src => src.CampusEntity.Campus_Name))
                .ForMember(x => x.addedBy, opt => opt.MapFrom(src => src.Added_By))
                .ForMember(x => x.updatedBy, opt => opt.MapFrom(src => src.Updated_By))
                .ForMember(x => x.isActive, opt => opt.MapFrom(src => src.IsActive))
                .ForMember(x => x.toDisplay, opt => opt.MapFrom(src => src.ToDisplay));

            CreateMap<officePagedResult, officePagedResultVM>();

            #endregion

            #region SchoolYearMapping

            CreateMap<schoolYearEntity, schoolYearVM>()
                .ForMember(x => x.schoolYearId, opt => opt.MapFrom(src => src.School_Year_ID))
                .ForMember(x => x.schoolYear, opt => opt.MapFrom(src => src.School_Year))
                .ForMember(x => x.startDate, opt => opt.MapFrom(src => src.Start_Date == null ? null : src.Start_Date.GetValueOrDefault().ToString("yyyy-MM-dd")))
                .ForMember(x => x.endDate, opt => opt.MapFrom(src => src.End_Date == null ? null : src.End_Date.GetValueOrDefault().ToString("yyyy-MM-dd")))
                .ForMember(x => x.addedBy, opt => opt.MapFrom(src => src.Added_By))
                .ForMember(x => x.updatedBy, opt => opt.MapFrom(src => src.Updated_By));

            CreateMap<schoolYearVM, schoolYearEntity>()
                .ForMember(x => x.School_Year_ID, opt => opt.MapFrom(src => src.schoolYearId))
                .ForMember(x => x.School_Year, opt => opt.MapFrom(src => src.schoolYear))
                .ForMember(x => x.Start_Date, opt => opt.MapFrom(src => src.startDate))
                .ForMember(x => x.End_Date, opt => opt.MapFrom(src => src.endDate))
                .ForMember(x => x.Added_By, opt => opt.MapFrom(src => src.addedBy))
                .ForMember(x => x.Updated_By, opt => opt.MapFrom(src => src.updatedBy));

            CreateMap<schoolYearPagedResult, schoolYearPagedResultVM>();
            
            #endregion

            #region TerminalMapping

            CreateMap<terminalVM, terminalEntity>()
                .ForMember(x => x.Terminal_ID, opt => opt.MapFrom(src => src.terminalId))
                .ForMember(x => x.Terminal_Name, opt => opt.MapFrom(src => Encryption.Encrypt(src.terminalName == null || src.terminalName == string.Empty ? src.mobileTerminalName : src.terminalName)))
                .ForMember(x => x.Terminal_Code, opt => opt.MapFrom(src => Encryption.Encrypt(src.terminalCode == null || src.terminalCode == string.Empty ? src.mobileTerminalCode : src.terminalCode)))
                .ForMember(x => x.Terminal_IP, opt => opt.MapFrom(src => Encryption.Encrypt(src.terminalIp == null || src.terminalIp == string.Empty ? src.mobileTerminalIp : src.terminalIp)))
                .ForMember(x => x.Terminal_Category, opt => opt.MapFrom(src => Encryption.Encrypt(src.terminalCategory)))
                .ForMember(x => x.Terminal_Status, opt => opt.MapFrom(src => src.terminalStatus == true ? "Active" : "Inactive"))
                .ForMember(x => x.Area_ID, opt => opt.MapFrom(src => src.areaId))
                .ForMember(x => x.Added_By, opt => opt.MapFrom(src => src.addedBy))
                .ForMember(x => x.Updated_By, opt => opt.MapFrom(src => src.updatedBy))
                .ForMember(x => x.IsActive, opt => opt.MapFrom(src => src.isActive))
                .ForMember(x => x.ToDisplay, opt => opt.MapFrom(src => src.toDisplay))
                ;

            CreateMap<terminalEntity, terminalVM>()
                .ForMember(x => x.terminalId, opt => opt.MapFrom(src => src.Terminal_ID))
                .ForMember(x => x.terminalName, opt => opt.MapFrom(src => Encryption.Decrypt(src.Terminal_Name)))
                .ForMember(x => x.terminalCode, opt => opt.MapFrom(src => Encryption.Decrypt(src.Terminal_Code)))
                .ForMember(x => x.terminalIp, opt => opt.MapFrom(src => Encryption.Decrypt(src.Terminal_IP)))
                .ForMember(x => x.terminalCategory, opt => opt.MapFrom(src => Encryption.Decrypt(src.Terminal_Category)))
                .ForMember(x => x.terminalStatus, opt => opt.MapFrom(src => src.Terminal_Status == "Active" ? true : false))
                .ForMember(x => x.areaId, opt => opt.MapFrom(src => src.Area_ID))
                .ForMember(x => x.areaName, opt => opt.MapFrom(src => src.AreaEntity.Area_Name))
                .ForMember(x => x.campusId, opt => opt.MapFrom(src => src.AreaEntity.Campus_ID))
                .ForMember(x => x.campusName, opt => opt.MapFrom(src => src.AreaEntity.CampusEntity.Campus_Name))
                .ForMember(x => x.currentCampus, opt => opt.MapFrom(src => src.AreaEntity.CampusEntity.Campus_Name))
                .ForMember(x => x.addedBy, opt => opt.MapFrom(src => src.Added_By))
                .ForMember(x => x.updatedBy, opt => opt.MapFrom(src => src.Updated_By))
                .ForMember(x => x.isActive, opt => opt.MapFrom(src => src.IsActive))
                .ForMember(x => x.toDisplay, opt => opt.MapFrom(src => src.ToDisplay))
                .ForMember(x => x.mobileTerminalName, opt => opt.MapFrom(src => Encryption.Decrypt(src.Terminal_Name)))
                .ForMember(x => x.mobileTerminalCode, opt => opt.MapFrom(src => Encryption.Decrypt(src.Terminal_Code)))
                .ForMember(x => x.mobileTerminalIp, opt => opt.MapFrom(src => Encryption.Decrypt(src.Terminal_IP)));

            CreateMap<terminalPagedResult, terminalPagedResultVM>();

            #endregion

            #region TerminalCategoryMapping

            CreateMap<terminalConfigurationEntity, terminalConfigVM>()
                    .ForMember(x => x.configId, opt => opt.MapFrom(src => src.Config_ID))
                    .ForMember(x => x.terminalCode, opt => opt.MapFrom(src => Encryption.Decrypt(src.Terminal_Code)))
                    .ForMember(x => x.terminalSchedule, opt => opt.MapFrom(src => src.Terminal_Schedule))
                    .ForMember(x => x.schoolName, opt => opt.MapFrom(src => src.School_Name))
                    .ForMember(x => x.hostIPAddress1, opt => opt.MapFrom(src => Encryption.Decrypt(src.Host_IPAddress1)))
                    .ForMember(x => x.hostPort1, opt => opt.MapFrom(src => src.Host_Port1))
                    .ForMember(x => x.hostIPAddress2, opt => opt.MapFrom(src => Encryption.Decrypt(src.Host_IPAddress2)))
                    .ForMember(x => x.hostPort2, opt => opt.MapFrom(src => src.Host_Port2))
                    .ForMember(x => x.viewerAddress, opt => opt.MapFrom(src => Encryption.Decrypt(src.Viewer_Address)))
                    .ForMember(x => x.viewerPort, opt => opt.MapFrom(src => src.Viewer_Port))
                    .ForMember(x => x.readerName1, opt => opt.MapFrom(src => Encryption.Decrypt(src.Reader_Name1)))
                    .ForMember(x => x.readerDirection1, opt => opt.MapFrom(src => (src.Reader_Direction1 == 2 ? "IN" : src.Reader_Direction1 == 1 ? "OUT" : src.Reader_Direction1 == 3 ? "OUT ONLY" : src.Reader_Direction1 == 4 ? "IN ONLY" : src.Reader_Direction1 == 6 ? "IN LIB" : src.Reader_Direction1 == 7 ? "OUT LIB" : "")))
                    .ForMember(x => x.enableAntipassback1, opt => opt.MapFrom(src => src.Enable_Antipassback1))
                    .ForMember(x => x.readerName2, opt => opt.MapFrom(src => Encryption.Decrypt(src.Reader_Name2)))
                    .ForMember(x => x.readerDirection2, opt => opt.MapFrom(src => (src.Reader_Direction2 == 2 ? "IN" : src.Reader_Direction2 == 1 ? "OUT" : src.Reader_Direction2 == 3 ? "OUT ONLY" : src.Reader_Direction2 == 4 ? "IN ONLY" : src.Reader_Direction2 == 6 ? "IN LIB" : src.Reader_Direction2 == 7 ? "OUT LIB" : "")))
                    .ForMember(x => x.enableAntipassback2, opt => opt.MapFrom(src => src.Enable_Antipassback2))
                    .ForMember(x => x.loopDelay, opt => opt.MapFrom(src => src.Loop_Delay))
                    .ForMember(x => x.turnstileDelay, opt => opt.MapFrom(src => src.Turnstile_Delay))
                    .ForMember(x => x.terminalSyncInterval, opt => opt.MapFrom(src => src.Terminal_Sync_Interval))
                    .ForMember(x => x.viewerDB, opt => opt.MapFrom(src => Encryption.Decrypt(src.Viewer_DB)))
                    .ForMember(x => x.terminalId, opt => opt.MapFrom(src => src.TerminalID))
                    .ForMember(x => x.terminalName, opt => opt.MapFrom(src => Encryption.Decrypt(src.TerminalEntity.Terminal_Name)))
                    .ForMember(x => x.areaId, opt => opt.MapFrom(src => src.TerminalEntity.Area_ID))
                    .ForMember(x => x.areaName, opt => opt.MapFrom(src => src.TerminalEntity.AreaEntity.Area_Name))
                    .ForMember(x => x.campusId, opt => opt.MapFrom(src => src.TerminalEntity.AreaEntity.Campus_ID))
                    .ForMember(x => x.campusName, opt => opt.MapFrom(src => src.TerminalEntity.AreaEntity.CampusEntity.Campus_Name))
                    .ForMember(x => x.serverDB, opt => opt.MapFrom(src => src.Server_DB));

            CreateMap<terminalConfigVM, terminalConfigurationEntity>()
                    .ForMember(x => x.Config_ID, opt => opt.MapFrom(src => src.configId))
                    .ForMember(x => x.Terminal_Code, opt => opt.MapFrom(src => Encryption.Encrypt(src.terminalCode)))
                    .ForMember(x => x.Terminal_Schedule, opt => opt.MapFrom(src => src.terminalSchedule))
                    .ForMember(x => x.School_Name, opt => opt.MapFrom(src => src.schoolName))
                    .ForMember(x => x.Host_IPAddress1, opt => opt.MapFrom(src => Encryption.Encrypt(src.hostIPAddress1)))
                    .ForMember(x => x.Host_Port1, opt => opt.MapFrom(src => src.hostPort1))
                    .ForMember(x => x.Host_IPAddress2, opt => opt.MapFrom(src => Encryption.Encrypt(src.hostIPAddress2)))
                    .ForMember(x => x.Host_Port2, opt => opt.MapFrom(src => src.hostPort2))
                    .ForMember(x => x.Viewer_Address, opt => opt.MapFrom(src => Encryption.Encrypt(src.viewerAddress)))
                    .ForMember(x => x.Viewer_Port, opt => opt.MapFrom(src => src.viewerPort))
                    .ForMember(x => x.Reader_Name1, opt => opt.MapFrom(src => Encryption.Encrypt(src.readerName1)))
                    .ForMember(x => x.Reader_Direction1, opt => opt.MapFrom(src => (src.readerDirection1.ToLower() == "in" ? 2 : src.readerDirection1.ToLower() == "out" ? 1 : src.readerDirection1.ToLower() == "in only" ? 4 : src.readerDirection1.ToLower() == "out only" ? 3 : src.readerDirection1.ToLower() == "in lib" ? 6 : src.readerDirection1.ToLower() == "out lib" ? 7 : 0)))
                    .ForMember(x => x.Enable_Antipassback1, opt => opt.MapFrom(src => src.enableAntipassback1))
                    .ForMember(x => x.Reader_Name2, opt => opt.MapFrom(src => Encryption.Encrypt(src.readerName2)))
                    .ForMember(x => x.Reader_Direction2, opt => opt.MapFrom(src => (src.readerDirection2.ToLower() == "in" ? 2 : src.readerDirection2.ToLower() == "out" ? 1 : src.readerDirection2.ToLower() == "in only" ? 4 : src.readerDirection2.ToLower() == "out only" ? 3 : src.readerDirection2.ToLower() == "in lib" ? 6 : src.readerDirection2.ToLower() == "out lib" ? 7 : 0)))
                    .ForMember(x => x.Enable_Antipassback2, opt => opt.MapFrom(src => src.enableAntipassback2))
                    .ForMember(x => x.Loop_Delay, opt => opt.MapFrom(src => src.loopDelay))
                    .ForMember(x => x.Turnstile_Delay, opt => opt.MapFrom(src => src.turnstileDelay))
                    .ForMember(x => x.Terminal_Sync_Interval, opt => opt.MapFrom(src => src.terminalSyncInterval))
                    .ForMember(x => x.Viewer_DB, opt => opt.MapFrom(src => Encryption.Encrypt(src.viewerDB)))
                    .ForMember(x => x.TerminalID, opt => opt.MapFrom(src => src.terminalId))
                    .ForMember(x => x.Server_DB, opt => opt.MapFrom(src => src.serverDB));


            #endregion

            #region RoleMapping

            CreateMap<roleEntity, roleVM>()
               .ForMember(x => x.status, opt => opt.MapFrom(src => src.IsActive == true ? "Active" : "Inactive"))
               .ForMember(x => x.role_ID, opt => opt.MapFrom(src => src.Role_ID))
               .ForMember(x => x.role_Name, opt => opt.MapFrom(src => src.Role_Name))
               .ForMember(x => x.isActive, opt => opt.MapFrom(src => src.IsActive))
               .ForMember(x => x.addedBy, opt => opt.MapFrom(src => src.Added_By))
               .ForMember(x => x.updatedBy, opt => opt.MapFrom(src => src.Updated_By))
               .ForMember(x => x.toDisplay, opt => opt.MapFrom(src => src.ToDisplay));


            CreateMap<roleVM, roleEntity>()
               .ForMember(x => x.Role_ID, opt => opt.MapFrom(src => src.role_ID));

            CreateMap<userAccessEntity, roleModuleVM>()
               .ForMember(x => x.Module_ID, opt => opt.MapFrom(src => src.Form_ID));

            CreateMap<rolePermissionEntity, roleModuleVM>()
               .ForMember(x => x.Module_ID, opt => opt.MapFrom(src => src.Form_ID));

            CreateMap<roleModuleVM, userAccessEntity>();

            CreateMap<userAccessEntity, roleAuthorised>()
                .ForMember(x => x.View, opt => opt.MapFrom(src => src.Can_Access))
                .ForMember(x => x.Insert, opt => opt.MapFrom(src => src.Can_Insert))
                .ForMember(x => x.Update, opt => opt.MapFrom(src => src.Can_Update))
                .ForMember(x => x.Delete, opt => opt.MapFrom(src => src.Can_Delete));

            CreateMap<roleAuthorised, userAccessEntity>()
                .ForMember(x => x.Can_Access, opt => opt.MapFrom(src => src.View))
                .ForMember(x => x.Can_Insert, opt => opt.MapFrom(src => src.Insert))
                .ForMember(x => x.Can_Update, opt => opt.MapFrom(src => src.Update))
                .ForMember(x => x.Can_Delete, opt => opt.MapFrom(src => src.Delete));

            CreateMap<rolePermissionEntity, roleAuthorised>()
                .ForMember(x => x.View, opt => opt.MapFrom(src => src.Can_Access))
                .ForMember(x => x.Insert, opt => opt.MapFrom(src => src.Can_Insert))
                .ForMember(x => x.Update, opt => opt.MapFrom(src => src.Can_Update))
                .ForMember(x => x.Delete, opt => opt.MapFrom(src => src.Can_Delete));

            CreateMap<roleAuthorised, rolePermissionEntity>()
                .ForMember(x => x.Can_Access, opt => opt.MapFrom(src => src.View))
                .ForMember(x => x.Can_Insert, opt => opt.MapFrom(src => src.Insert))
                .ForMember(x => x.Can_Update, opt => opt.MapFrom(src => src.Update))
                .ForMember(x => x.Can_Delete, opt => opt.MapFrom(src => src.Delete));
            
            #endregion

            #region UserMapping

            CreateMap<userEntity, userAuditTrailVM>()
                .ForMember(x => x.user_ID, opt => opt.MapFrom(src => src.User_ID))
                .ForMember(x => x.full_Info, opt => opt.MapFrom(src => string.Concat(src.PersonEntity.ID_Number, " : ", src.PersonEntity.First_Name, " ", src.PersonEntity.Last_Name)));

            //CreateMap<userEntity, userVM>()
            //   .ForMember(x => x.Status, opt => opt.MapFrom(src => src.IsActive == true ? "Active" : "Inactive"))
            //   .ForMember(x => x.Role_ID, opt => opt.MapFrom(src => src.UserRoleEntity.RoleEntity.Role_ID))
            //   .ForMember(x => x.Role_Name, opt => opt.MapFrom(src => src.UserRoleEntity.RoleEntity.Role_Name))
            //   .ForMember(x => x.Email_Address, opt => opt.MapFrom(src => src.PersonEntity.Email_Address))
            //   .ForMember(x => x.Fullname, opt => opt.MapFrom(src => src.PersonEntity.Last_Name + " " + src.PersonEntity.First_Name + ", " + src.PersonEntity.Middle_Name))
            //   .ForMember(x => x.Id_Number, opt => opt.MapFrom(src => src.PersonEntity.ID_Number))
            //   .ForMember(x => x.Department_Name, opt => opt.MapFrom(src => src.PersonEntity.PositionEntity.DepartmentEntity.Department_Name))
            //   .ForMember(x => x.Position_Name, opt => opt.MapFrom(src => src.PersonEntity.PositionEntity.Position_Name))
            //   .ForMember(x => x.User_ID, opt => opt.MapFrom(src => src.User_ID))
            //   .ForMember(x => x.User_Name, opt => opt.MapFrom(src => src.User_Name)); 

            CreateMap<userEntity, userVM>()
               .ForMember(x => x.Status, opt => opt.MapFrom(src => src.IsActive == true ? "Active" : "Inactive"))
               .ForMember(x => x.Role_ID, opt => opt.MapFrom(src => src.RoleEntity.Role_ID))
               .ForMember(x => x.Role_Name, opt => opt.MapFrom(src => src.RoleEntity.Role_Name))
               .ForMember(x => x.Email_Address, opt => opt.MapFrom(src => src.PersonEntity.Email_Address))
               .ForMember(x => x.Fullname, opt => opt.MapFrom(src => src.PersonEntity.Last_Name + " " + src.PersonEntity.First_Name + ", " + src.PersonEntity.Middle_Name))
               .ForMember(x => x.Id_Number, opt => opt.MapFrom(src => src.PersonEntity.ID_Number))
               .ForMember(x => x.Department_Name, opt => opt.MapFrom(src => src.PersonEntity.PositionEntity.DepartmentEntity.Department_Name))
               .ForMember(x => x.Position_Name, opt => opt.MapFrom(src => src.PersonEntity.PositionEntity.Position_Name))
               .ForMember(x => x.User_ID, opt => opt.MapFrom(src => src.User_ID))
               .ForMember(x => x.User_Name, opt => opt.MapFrom(src => src.User_Name));

            CreateMap<userVM, userEntity>();

            CreateMap<userVM, userRoleEntity>()
                .ForMember(x => x.Role_ID, opt => opt.MapFrom(src => src.Role_ID))
                .ForMember(x => x.User_ID, opt => opt.MapFrom(src => src.User_ID));

            //CreateMap<userEntity, userRoleVM>()
            //    .ForMember(x => x.user_role_ID, opt => opt.MapFrom(src => src.UserRoleEntity.UserRole_ID))
            //    .ForMember(x => x.role_ID, opt => opt.MapFrom(src => src.UserRoleEntity.RoleEntity.Role_ID))
            //    .ForMember(x => x.role_Name, opt => opt.MapFrom(src => src.UserRoleEntity.RoleEntity.Role_Name))
            //    .ForMember(x => x.user_ID, opt => opt.MapFrom(src => src.User_ID))
            //    .ForMember(x => x.user_Name, opt => opt.MapFrom(src => src.User_Name))
            //    .ForMember(x => x.id_Number, opt => opt.MapFrom(src => src.PersonEntity.ID_Number))
            //    .ForMember(x => x.full_Name, opt => opt.MapFrom(src => string.Concat(src.PersonEntity.Last_Name, ", ", src.PersonEntity.First_Name)))
            //    .ForMember(x => x.position_Name, opt => opt.MapFrom(src => src.PersonEntity.PositionEntity.Position_Name))
            //    .ForMember(x => x.department_Name, opt => opt.MapFrom(src => src.PersonEntity.PositionEntity.DepartmentEntity.Department_Name))
            //    .ForMember(x => x.campus_Name, opt => opt.MapFrom(src => src.PersonEntity.CampusEntity.Campus_Name));

            CreateMap<userEntity, userRoleVM>()
                .ForMember(x => x.role_ID, opt => opt.MapFrom(src => src.RoleEntity.Role_ID))
                .ForMember(x => x.role_Name, opt => opt.MapFrom(src => src.RoleEntity.Role_Name))
                .ForMember(x => x.user_ID, opt => opt.MapFrom(src => src.User_ID))
                .ForMember(x => x.user_Name, opt => opt.MapFrom(src => src.User_Name))
                .ForMember(x => x.id_Number, opt => opt.MapFrom(src => src.PersonEntity.ID_Number))
                .ForMember(x => x.full_Name, opt => opt.MapFrom(src => string.Concat(src.PersonEntity.Last_Name, ", ", src.PersonEntity.First_Name)))
                .ForMember(x => x.position_Name, opt => opt.MapFrom(src => src.PersonEntity.PositionEntity.Position_Name))
                .ForMember(x => x.department_Name, opt => opt.MapFrom(src => src.PersonEntity.PositionEntity.DepartmentEntity.Department_Name))
                .ForMember(x => x.campus_Name, opt => opt.MapFrom(src => src.PersonEntity.CampusEntity.Campus_Name));

            //CreateMap<userRoleVM, userRoleEntity>()
            //    .ForMember(x => x.UserRole_ID, opt => opt.MapFrom(src => src.user_role_ID))
            //    .ForMember(x => x.Role_ID, opt => opt.MapFrom(src => src.role_ID))
            //    .ForMember(x => x.User_ID, opt => opt.MapFrom(src => src.user_ID));
            
            CreateMap<rolePermissionEntity, roleEntity>()
                .ForMember(x => x.Role_ID, opt => opt.MapFrom(src => src.Role_ID));

            #endregion

            #region EmployeeMapping

            CreateMap<personEntity, employeeVM>()
                .ForMember(x => x.address, opt => opt.MapFrom(src => src.Address))
                .ForMember(x => x.birthdate, opt => opt.MapFrom(src => src.Birthdate.ToString("yyyy-MM-dd")))
                .ForMember(x => x.contactNumber, opt => opt.MapFrom(src => src.Contact_Number))
                .ForMember(x => x.telephoneNumber, opt => opt.MapFrom(src => src.Telephone_Number == null || src.Telephone_Number == "undefined" ? "" : src.Telephone_Number))
                .ForMember(x => x.gender, opt => opt.MapFrom(src => (src.Gender.ToLower() == "m" ? "Male" : "Female")))
                .ForMember(x => x.emailAddress, opt => opt.MapFrom(src => src.Email_Address == null || src.Email_Address == "undefined" ? "" : src.Email_Address))
                .ForMember(x => x.firstName, opt => opt.MapFrom(src => src.First_Name))
                .ForMember(x => x.lastName, opt => opt.MapFrom(src => src.Last_Name))
                .ForMember(x => x.middleName, opt => opt.MapFrom(src => src.Middle_Name == null || src.Middle_Name == "undefined" ? "" : src.Middle_Name))
                .ForMember(x => x.idNumber, opt => opt.MapFrom(src => src.ID_Number))
                .ForMember(x => x.departmentName, opt => opt.MapFrom(src => src.DepartmentEntity.Department_Name))
                .ForMember(x => x.positionName, opt => opt.MapFrom(src => src.PositionEntity.Position_Name))
                .ForMember(x => x.employeeSubTypeDesc, opt => opt.MapFrom(src => src.EmployeeSubTypeEntity.EmpSubTypeDesc))
                .ForMember(x => x.employeeTypeDesc, opt => opt.MapFrom(src => src.EmployeeTypeEntity.EmpTypeDesc))
                .ForMember(x => x.campusName, opt => opt.MapFrom(src => src.CampusEntity.Campus_Name))
                .ForMember(x => x.campusId, opt => opt.MapFrom(src => src.Campus_ID))
                .ForMember(x => x.departmentId, opt => opt.MapFrom(src => src.Department_ID))
                .ForMember(x => x.positionId, opt => opt.MapFrom(src => src.Position_ID))
                .ForMember(x => x.employeeTypeId, opt => opt.MapFrom(src => src.EmpType_ID))
                .ForMember(x => x.employeeSubTypeId, opt => opt.MapFrom(src => src.EmpSubtype_ID))
                .ForMember(x => x.emergencyAddress, opt => opt.MapFrom(src => src.EmergencyContactEntity.Address))
                .ForMember(x => x.emergencyFullname, opt => opt.MapFrom(src => src.EmergencyContactEntity.Full_Name))
                .ForMember(x => x.emergencyRelationship, opt => opt.MapFrom(src => src.EmergencyContactEntity.Relationship))
                .ForMember(x => x.emergencyContact, opt => opt.MapFrom(src => src.EmergencyContactEntity.Contact_Number))
                .ForMember(x => x.sss, opt => opt.MapFrom(src => src.GovIdsEntity.SSS))
                .ForMember(x => x.pagibig, opt => opt.MapFrom(src => src.GovIdsEntity.PAG_IBIG))
                .ForMember(x => x.philhealth, opt => opt.MapFrom(src => src.GovIdsEntity.PhilHealth))
                .ForMember(x => x.tin, opt => opt.MapFrom(src => src.GovIdsEntity.TIN))
                .ForMember(x => x.personId, opt => opt.MapFrom(src => src.Person_ID))
                .ForMember(x => x.status, opt => opt.MapFrom(src => src.IsActive == true ? "Active" : "Inactive"));

            CreateMap<employeeVM, personEntity>()
                .ForMember(x => x.Address, opt => opt.MapFrom(src => src.address))
                .ForMember(x => x.Birthdate, opt => opt.MapFrom(src => src.birthdate))
                .ForMember(x => x.Contact_Number, opt => opt.MapFrom(src => src.contactNumber))
                .ForMember(x => x.Telephone_Number, opt => opt.MapFrom(src => src.telephoneNumber == null || src.telephoneNumber == "undefined" ? "" : src.telephoneNumber))
                .ForMember(x => x.Email_Address, opt => opt.MapFrom(src => src.emailAddress))
                .ForMember(x => x.Gender, opt => opt.MapFrom(src => src.gender))
                .ForMember(x => x.First_Name, opt => opt.MapFrom(src => src.firstName))
                .ForMember(x => x.Last_Name, opt => opt.MapFrom(src => src.lastName))
                .ForMember(x => x.Middle_Name, opt => opt.MapFrom(src => src.middleName == null || src.middleName == "undefined" ? "" : src.middleName))
                .ForMember(x => x.ID_Number, opt => opt.MapFrom(src => src.idNumber))
                .ForMember(x => x.Campus_ID, opt => opt.MapFrom(src => src.campusId))
                .ForMember(x => x.Department_ID, opt => opt.MapFrom(src => src.departmentId))
                .ForMember(x => x.Position_ID, opt => opt.MapFrom(src => src.positionId))
                .ForMember(x => x.EmpType_ID, opt => opt.MapFrom(src => src.employeeTypeId))
                .ForMember(x => x.EmpSubtype_ID, opt => opt.MapFrom(src => src.employeeSubTypeId))
                .ForMember(x => x.Person_ID, opt => opt.MapFrom(src => src.personId))
                .ForMember(x => x.IsActive, opt => opt.MapFrom(src => src.status.Equals("1") ? true : false));

            CreateMap<employeePagedResult, employeePagedResultVM>();

            CreateMap<personEntity, employeeTableVM>()
                .ForMember(x => x.firstName, opt => opt.MapFrom(src => src.First_Name))
                .ForMember(x => x.lastName, opt => opt.MapFrom(src => src.Last_Name))
                .ForMember(x => x.middleName, opt => opt.MapFrom(src => src.Middle_Name == null ? "" : src.Middle_Name))
                .ForMember(x => x.idNumber, opt => opt.MapFrom(src => src.ID_Number))
                .ForMember(x => x.employeeTypeDesc, opt => opt.MapFrom(src => src.EmployeeTypeEntity.EmpTypeDesc))
                .ForMember(x => x.employeeSubTypeDesc, opt => opt.MapFrom(src => src.EmployeeSubTypeEntity.EmpSubTypeDesc))
                .ForMember(x => x.departmentName, opt => opt.MapFrom(src => src.PositionEntity.DepartmentEntity.Department_Name))
                .ForMember(x => x.positionName, opt => opt.MapFrom(src => src.PositionEntity.Position_Name))
                .ForMember(x => x.campusName, opt => opt.MapFrom(src => src.PositionEntity.DepartmentEntity.CampusEntity.Campus_Name));

            CreateMap<personPagedResult, personPagedResultVM>();

            CreateMap<personEntity, employeeTableVM>()
                .ForMember(x => x.address, opt => opt.MapFrom(src => src.Address))
                .ForMember(x => x.birthdate, opt => opt.MapFrom(src => src.Birthdate.ToString("yyyy-MM-dd")))
                .ForMember(x => x.contactNumber, opt => opt.MapFrom(src => src.Contact_Number))
                .ForMember(x => x.telephoneNumber, opt => opt.MapFrom(src => src.Telephone_Number))
                .ForMember(x => x.gender, opt => opt.MapFrom(src => src.Gender))
                .ForMember(x => x.emailAddress, opt => opt.MapFrom(src => src.Email_Address))
                .ForMember(x => x.firstName, opt => opt.MapFrom(src => src.First_Name))
                .ForMember(x => x.lastName, opt => opt.MapFrom(src => src.Last_Name))
                .ForMember(x => x.middleName, opt => opt.MapFrom(src => src.Middle_Name))
                .ForMember(x => x.idNumber, opt => opt.MapFrom(src => src.ID_Number))
                .ForMember(x => x.departmentName, opt => opt.MapFrom(src => src.DepartmentEntity.Department_Name))
                .ForMember(x => x.positionName, opt => opt.MapFrom(src => src.PositionEntity.Position_Name))
                .ForMember(x => x.employeeSubTypeDesc, opt => opt.MapFrom(src => src.EmployeeSubTypeEntity.EmpSubTypeDesc))
                .ForMember(x => x.employeeTypeDesc, opt => opt.MapFrom(src => src.EmployeeTypeEntity.EmpTypeDesc))
                .ForMember(x => x.campusName, opt => opt.MapFrom(src => src.CampusEntity.Campus_Name))
                .ForMember(x => x.campusId, opt => opt.MapFrom(src => src.Campus_ID))
                .ForMember(x => x.departmentId, opt => opt.MapFrom(src => src.Department_ID))
                .ForMember(x => x.positionId, opt => opt.MapFrom(src => src.Position_ID))
                .ForMember(x => x.employeeTypeId, opt => opt.MapFrom(src => src.EmpType_ID))
                .ForMember(x => x.employeeSubTypeId, opt => opt.MapFrom(src => src.EmpSubtype_ID))
                .ForMember(x => x.emergencyAddress, opt => opt.MapFrom(src => src.EmergencyContactEntity.Address))
                .ForMember(x => x.emergencyFullname, opt => opt.MapFrom(src => src.EmergencyContactEntity.Full_Name))
                .ForMember(x => x.emergencyRelationship, opt => opt.MapFrom(src => src.EmergencyContactEntity.Relationship))
                .ForMember(x => x.emergencyContact, opt => opt.MapFrom(src => src.EmergencyContactEntity.Contact_Number))
                .ForMember(x => x.sss, opt => opt.MapFrom(src => src.GovIdsEntity.SSS))
                .ForMember(x => x.pagibig, opt => opt.MapFrom(src => src.GovIdsEntity.PAG_IBIG))
                .ForMember(x => x.philhealth, opt => opt.MapFrom(src => src.GovIdsEntity.PhilHealth))
                .ForMember(x => x.tin, opt => opt.MapFrom(src => src.GovIdsEntity.TIN))
                .ForMember(x => x.personId, opt => opt.MapFrom(src => src.Person_ID))
                .ForMember(x => x.status, opt => opt.MapFrom(src => src.IsActive == true ? "Active" : "Inactive"));

            CreateMap<personEmployeeBatchUploadHeaders, personEmployeeBatchUploadVM>()
                .ForMember(x => x.IDNumber, opt => opt.MapFrom(src => src.IDNumber))
                .ForMember(x => x.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(x => x.MiddleName, opt => opt.MapFrom(src => src.MiddleName))
                .ForMember(x => x.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(x => x.Gender, opt => opt.MapFrom(src => src.Gender))
                .ForMember(x => x.BirthDate, opt => opt.MapFrom(src => Convert.ToDateTime(src.BirthDate).ToString("yyyy-MM-dd")))
                .ForMember(x => x.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(x => x.EmailAddress, opt => opt.MapFrom(src => src.EmailAddress))
                .ForMember(x => x.ContactNumber, opt => opt.MapFrom(src => src.ContactNumber))
                .ForMember(x => x.TelephoneNumber, opt => opt.MapFrom(src => src.TelephoneNumber))
                .ForMember(x => x.PositionName, opt => opt.MapFrom(src => src.PositionName))
                .ForMember(x => x.DepartmentName, opt => opt.MapFrom(src => src.DepartmentName))
                .ForMember(x => x.CampusName, opt => opt.MapFrom(src => src.CampusName))
                .ForMember(x => x.EmergencyFullname, opt => opt.MapFrom(src => src.EmergencyFullname))
                .ForMember(x => x.EmergencyMobileNo, opt => opt.MapFrom(src => src.EmergencyMobileNo))
                .ForMember(x => x.EmergencyRelationship, opt => opt.MapFrom(src => src.EmergencyRelationship))
                .ForMember(x => x.EmergencyAddress, opt => opt.MapFrom(src => src.EmergencyAddress))
                .ForMember(x => x.SSS, opt => opt.MapFrom(src => src.SSS))
                .ForMember(x => x.PAGIBIG, opt => opt.MapFrom(src => src.PAGIBIG))
                .ForMember(x => x.TIN, opt => opt.MapFrom(src => src.TIN))
                .ForMember(x => x.Philhealth, opt => opt.MapFrom(src => src.Philhealth));

            CreateMap<personEntity, personVM>()
               .ForMember(x => x.personType, opt => opt.MapFrom(src => src.Person_Type == "E" ? "Employee" : src.Person_Type == "S" ? "Student" 
                                                                                                            : src.Person_Type == "O" ? "Other Access"
                                                                                                            : src.Person_Type == "V" ? "Visitor"
                                                                                                            : src.Person_Type == "F" ? "Fetcher" 
                                                                                                            : src.Person_Type == "A" ? "Admin" : ""))
               .ForMember(x => x.personID, opt => opt.MapFrom(src => src.Person_ID))
               .ForMember(x => x.idNumber, opt => opt.MapFrom(src => src.ID_Number))
               .ForMember(x => x.firstName, opt => opt.MapFrom(src => src.First_Name))
               .ForMember(x => x.middleName, opt => opt.MapFrom(src => src.Middle_Name))
               .ForMember(x => x.lastName, opt => opt.MapFrom(src => src.Last_Name))
               .ForMember(x => x.birthdate, opt => opt.MapFrom(src => src.Birthdate))
               .ForMember(x => x.gender, opt => opt.MapFrom(src => src.Gender))
               .ForMember(x => x.address, opt => opt.MapFrom(src => src.Address));

            CreateMap<personVM, personEntity>();



            #endregion

            #region StudentMapping

            CreateMap<personEntity, studentVM>()
                .ForMember(x => x.address, opt => opt.MapFrom(src => src.Address))
                .ForMember(x => x.birthdate, opt => opt.MapFrom(src => src.Birthdate.ToString("yyyy-MM-dd")))
                .ForMember(x => x.contactNumber, opt => opt.MapFrom(src => src.Contact_Number == null || src.Contact_Number == "undefined" ? "" : src.Contact_Number))
                .ForMember(x => x.telephoneNumber, opt => opt.MapFrom(src => src.Telephone_Number == null || src.Telephone_Number == "undefined" ? "" : src.Telephone_Number))
                .ForMember(x => x.gender, opt => opt.MapFrom(src => (src.Gender.ToLower() == "m" ? "Male" : "Female")))
                .ForMember(x => x.emailAddress, opt => opt.MapFrom(src => src.Email_Address == null || src.Email_Address == "undefined" ? "" : src.Email_Address))
                .ForMember(x => x.firstName, opt => opt.MapFrom(src => src.First_Name))
                .ForMember(x => x.lastName, opt => opt.MapFrom(src => src.Last_Name))
                .ForMember(x => x.middleName, opt => opt.MapFrom(src => src.Middle_Name == null || src.Middle_Name == "undefined" ? "" : src.Middle_Name))
                .ForMember(x => x.idNumber, opt => opt.MapFrom(src => src.ID_Number))
                .ForMember(x => x.campusName, opt => opt.MapFrom(src => src.CampusEntity.Campus_Name))
                .ForMember(x => x.campusId, opt => opt.MapFrom(src => src.Campus_ID))
                .ForMember(x => x.educLevelId, opt => opt.MapFrom(src => src.Educ_Level_ID))
                .ForMember(x => x.educLevelName, opt => opt.MapFrom(src => src.EducationalLevelEntity.Level_Name))
                .ForMember(x => x.yearSecId, opt => opt.MapFrom(src => src.Year_Section_ID))
                .ForMember(x => x.yearSecName, opt => opt.MapFrom(src => src.YearSectionEntity.YearSec_Name))
                .ForMember(x => x.studSecId, opt => opt.MapFrom(src => src.StudSec_ID))
                .ForMember(x => x.description, opt => opt.MapFrom(src => src.StudentSectionEntity.Description))
                .ForMember(x => x.collegeId, opt => opt.MapFrom(src => src.College_ID))
                .ForMember(x => x.collegeName, opt => opt.MapFrom(src => src.CollegeEntity.College_Name))
                .ForMember(x => x.courseId, opt => opt.MapFrom(src => src.Course_ID))
                .ForMember(x => x.courseName, opt => opt.MapFrom(src => src.CourseEntity.Course_Name))
                .ForMember(x => x.emergencyAddress, opt => opt.MapFrom(src => src.EmergencyContactEntity.Address))
                .ForMember(x => x.emergencyFullname, opt => opt.MapFrom(src => src.EmergencyContactEntity.Full_Name))
                .ForMember(x => x.emergencyRelationship, opt => opt.MapFrom(src => src.EmergencyContactEntity.Relationship))
                .ForMember(x => x.emergencyContact, opt => opt.MapFrom(src => src.EmergencyContactEntity.Contact_Number))
                .ForMember(x => x.personId, opt => opt.MapFrom(src => src.Person_ID))
                .ForMember(x => x.status, opt => opt.MapFrom(src => src.IsActive == true ? "Active" : "Inactive"))
                .ForMember(x => x.dateEnrolled, opt => opt.MapFrom(src => src.DateEnrolled.ToString("yyyy-MM-dd")))
                .ForMember(x => x.isDropOut, opt => opt.MapFrom(src => src.IsDropOut))
                .ForMember(x => x.dropOutCode, opt => opt.MapFrom(src => src.DropOutCode))
                .ForMember(x => x.isTransferred, opt => opt.MapFrom(src => src.IsTransferred))
                .ForMember(x => x.transferredSchoolName, opt => opt.MapFrom(src => src.TransferredSchoolName))
                .ForMember(x => x.dropOutOtherRemark, opt => opt.MapFrom(src => src.DropOutOtherRemark))
                .ForMember(x => x.isTransferredIn, opt => opt.MapFrom(src => src.IsTransferredIn))
                .ForMember(x => x.transferredInSchoolName, opt => opt.MapFrom(src => src.TransferredInSchoolName))
                .ForMember(x => x.educStatus, opt => opt.MapFrom(src => src.IsDropOut ? "Dropped Out" : (src.IsTransferred ? "Transferred Out" : (src.IsTransferredIn ? "Transferred In" : ""))))
                .ForMember(x => x.dropOutDesc, opt => opt.MapFrom(src => src.DropOutCode == null ? "" : src.DropOutCode))
                .ForMember(x => x.dropOutRemarks, opt => opt.MapFrom(src => src.DropOutOtherRemark == null ? "" : src.DropOutOtherRemark))
                .ForMember(x => x.schoolName, opt => opt.MapFrom(src => src.IsTransferred ? src.TransferredSchoolName : src.IsTransferredIn ? src.TransferredInSchoolName : ""));

            CreateMap<studentVM, personEntity>()
                .ForMember(x => x.Address, opt => opt.MapFrom(src => src.address))
                .ForMember(x => x.Birthdate, opt => opt.MapFrom(src => src.birthdate))
                .ForMember(x => x.Contact_Number, opt => opt.MapFrom(src => src.contactNumber == null || src.contactNumber == "undefined" ? "" : src.contactNumber))
                .ForMember(x => x.Telephone_Number, opt => opt.MapFrom(src => src.telephoneNumber == null || src.telephoneNumber == "undefined" ? "" : src.telephoneNumber))
                .ForMember(x => x.Gender, opt => opt.MapFrom(src => src.gender))
                .ForMember(x => x.Email_Address, opt => opt.MapFrom(src => src.emailAddress == null || src.emailAddress == "undefined" ? "" : src.emailAddress))
                .ForMember(x => x.First_Name, opt => opt.MapFrom(src => src.firstName))
                .ForMember(x => x.Last_Name, opt => opt.MapFrom(src => src.lastName))
                .ForMember(x => x.Middle_Name, opt => opt.MapFrom(src => src.middleName == null || src.middleName == "undefined" ? "" : src.middleName))
                .ForMember(x => x.ID_Number, opt => opt.MapFrom(src => src.idNumber))
                .ForMember(x => x.Educ_Level_ID, opt => opt.MapFrom(src => src.educLevelId))
                .ForMember(x => x.Year_Section_ID, opt => opt.MapFrom(src => src.yearSecId))
                .ForMember(x => x.Course_ID, opt => opt.MapFrom(src => src.courseId))
                .ForMember(x => x.College_ID, opt => opt.MapFrom(src => src.collegeId))
                .ForMember(x => x.StudSec_ID, opt => opt.MapFrom(src => src.studSecId))
                .ForMember(x => x.Campus_ID, opt => opt.MapFrom(src => src.campusId))
                .ForMember(x => x.Person_ID, opt => opt.MapFrom(src => src.personId))
                .ForMember(x => x.IsActive, opt => opt.MapFrom(src => src.status.Equals("1") ? true : false))
                .ForMember(x => x.DateEnrolled, opt => opt.MapFrom(src => src.dateEnrolled))
                .ForMember(x => x.IsDropOut, opt => opt.MapFrom(src => src.isDropOut.Equals("1") ? true : false))
                .ForMember(x => x.DropOutCode, opt => opt.MapFrom(src => src.dropOutCode))
                .ForMember(x => x.IsTransferred, opt => opt.MapFrom(src => src.isTransferred.Equals("1") ? true : false))
                .ForMember(x => x.TransferredSchoolName, opt => opt.MapFrom(src => src.transferredSchoolName))
                .ForMember(x => x.DropOutOtherRemark, opt => opt.MapFrom(src => src.dropOutOtherRemark))
                .ForMember(x => x.IsTransferredIn, opt => opt.MapFrom(src => src.isTransferredIn.Equals("1") ? true : false))
                .ForMember(x => x.EducStatus, opt => opt.MapFrom(src => src.educStatus))
                .ForMember(x => x.DropOutType, opt => opt.MapFrom(src => src.dropOutType))
                .ForMember(x => x.DropOutDesc, opt => opt.MapFrom(src => src.dropOutDesc))
                .ForMember(x => x.DropOutRemarks, opt => opt.MapFrom(src => src.dropOutRemarks))
                .ForMember(x => x.SchoolName, opt => opt.MapFrom(src => src.schoolName));

            CreateMap<excusedStudentEntity, excuseVM>()
                .ForMember(x => x.id, opt => opt.MapFrom(src => src.ID))
                .ForMember(x => x.excuseIndex, opt => opt.MapFrom(src => src.IDNumber))
                .ForMember(x => x.excusedDate, opt => opt.MapFrom(src => src.Excused_Date.ToString("yyyy-MM-dd")))
                .ForMember(x => x.excusedStartDate, opt => opt.MapFrom(src => src.Excused_Start_Date.ToString("yyyy-MM-dd")))
                .ForMember(x => x.excusedEndDate, opt => opt.MapFrom(src => src.Excused_End_Date.ToString("yyyy-MM-dd")));

            CreateMap<excuseVM, excusedStudentEntity>()
                .ForMember(x => x.ID, opt => opt.MapFrom(src => src.id))
                .ForMember(x => x.IDNumber, opt => opt.MapFrom(src => src.excuseIndex))
                .ForMember(x => x.Excused_Date, opt => opt.MapFrom(src => src.excusedDate))
                .ForMember(x => x.Excused_Start_Date, opt => opt.MapFrom(src => src.excusedStartDate))
                .ForMember(x => x.Excused_End_Date, opt => opt.MapFrom(src => src.excusedEndDate));

            CreateMap<dropoutCodeEntity, dropoutCodeVM>()
                .ForMember(x => x.id, opt => opt.MapFrom(src => src.ID))
                .ForMember(x => x.code, opt => opt.MapFrom(src => src.Code))
                .ForMember(x => x.name, opt => opt.MapFrom(src => src.Name))
                .ForMember(x => x.description, opt => opt.MapFrom(src => src.Description));

            CreateMap<dropoutCodeVM, dropoutCodeEntity>()
                .ForMember(x => x.ID, opt => opt.MapFrom(src => src.id))
                .ForMember(x => x.Code, opt => opt.MapFrom(src => src.code))
                .ForMember(x => x.Name, opt => opt.MapFrom(src => src.name))
                .ForMember(x => x.Description, opt => opt.MapFrom(src => src.description));
            
            CreateMap<dropoutCodeEntity, personEntity>()
                .ForMember(x => x.DropOutCode, opt => opt.MapFrom(src => src.Description));

            CreateMap<personStudentBatchUploadHeaders, personStudentBatchUploadVM>()
                .ForMember(x => x.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(x => x.BirthDate, opt => opt.MapFrom(src => Convert.ToDateTime(src.BirthDate).ToString("yyyy-MM-dd")))
                .ForMember(x => x.ContactNumber, opt => opt.MapFrom(src => src.ContactNumber == null || src.ContactNumber == "undefined" ? "" : src.ContactNumber))
                .ForMember(x => x.TelephoneNumber, opt => opt.MapFrom(src => src.TelephoneNumber == null || src.TelephoneNumber == "undefined" ? "" : src.TelephoneNumber))
                .ForMember(x => x.Gender, opt => opt.MapFrom(src => src.Gender))
                .ForMember(x => x.EmailAddress, opt => opt.MapFrom(src => src.EmailAddress))
                .ForMember(x => x.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(x => x.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(x => x.MiddleName, opt => opt.MapFrom(src => src.MiddleName))
                .ForMember(x => x.IDNumber, opt => opt.MapFrom(src => src.IDNumber))
                .ForMember(x => x.EducationalLevelName, opt => opt.MapFrom(src => src.EducationalLevelName))
                .ForMember(x => x.YearLevelName, opt => opt.MapFrom(src => src.YearLevelName))
                .ForMember(x => x.SectionName, opt => opt.MapFrom(src => src.SectionName))
                .ForMember(x => x.CourseName, opt => opt.MapFrom(src => src.CourseName))
                .ForMember(x => x.CollegeName, opt => opt.MapFrom(src => src.CollegeName))
                .ForMember(x => x.CampusName, opt => opt.MapFrom(src => src.CampusName))
                .ForMember(x => x.EmergencyAddress, opt => opt.MapFrom(src => src.EmailAddress))
                .ForMember(x => x.EmergencyFullname, opt => opt.MapFrom(src => src.EmergencyFullname))
                .ForMember(x => x.EmergencyRelationship, opt => opt.MapFrom(src => src.EmergencyRelationship))
                .ForMember(x => x.EmergencyContactNo, opt => opt.MapFrom(src => src.EmergencyContactNo))
                .ForMember(x => x.DateEnrolled, opt => opt.MapFrom(src => Convert.ToDateTime(src.DateEnrolled).ToString("yyyy-MM-dd")))
                .ForMember(x => x.IsDropOut, opt => opt.MapFrom(src => src.IsDropOut))
                .ForMember(x => x.DropOutCode, opt => opt.MapFrom(src => src.DropOutCode))
                .ForMember(x => x.IsTransferred, opt => opt.MapFrom(src => src.IsTransferred))
                .ForMember(x => x.TransferredSchoolName, opt => opt.MapFrom(src => src.TransferredSchoolName))
                .ForMember(x => x.DropOutOtherRemark, opt => opt.MapFrom(src => src.DropOutOtherRemark))
                .ForMember(x => x.IsTransferredIn, opt => opt.MapFrom(src => src.IsTransferredIn))
                .ForMember(x => x.TransferredInSchoolName, opt => opt.MapFrom(src => src.TransferredInSchoolName));

            CreateMap<personStudentBatchUploadHeaders, personStudentBatchUploadVM>()
                .ForMember(x => x.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(x => x.BirthDate, opt => opt.MapFrom(src => Convert.ToDateTime(src.BirthDate).ToString("yyyy-MM-dd")))
                .ForMember(x => x.ContactNumber, opt => opt.MapFrom(src => src.ContactNumber == null || src.ContactNumber == "undefined" ? "" : src.ContactNumber))
                .ForMember(x => x.Gender, opt => opt.MapFrom(src => src.Gender))
                .ForMember(x => x.EmailAddress, opt => opt.MapFrom(src => src.EmailAddress))
                .ForMember(x => x.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(x => x.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(x => x.MiddleName  , opt => opt.MapFrom(src => src.MiddleName))
                .ForMember(x => x.IDNumber, opt => opt.MapFrom(src => src.IDNumber))
                .ForMember(x => x.EducationalLevelName, opt => opt.MapFrom(src => src.EducationalLevelName))
                .ForMember(x => x.YearLevelName, opt => opt.MapFrom(src => src.YearLevelName))
                .ForMember(x => x.SectionName, opt => opt.MapFrom(src => src.SectionName))
                .ForMember(x => x.CourseName, opt => opt.MapFrom(src => src.CourseName))
                .ForMember(x => x.CollegeName, opt => opt.MapFrom(src => src.CollegeName))
                .ForMember(x => x.CampusName, opt => opt.MapFrom(src => src.CampusName))
                .ForMember(x => x.EmergencyAddress, opt => opt.MapFrom(src => src.EmailAddress))
                .ForMember(x => x.EmergencyFullname, opt => opt.MapFrom(src => src.EmergencyFullname))
                .ForMember(x => x.EmergencyRelationship, opt => opt.MapFrom(src => src.EmergencyRelationship))
                .ForMember(x => x.EmergencyContactNo, opt => opt.MapFrom(src => src.EmergencyContactNo))
                .ForMember(x => x.DateEnrolled, opt => opt.MapFrom(src => Convert.ToDateTime(src.DateEnrolled).ToString("yyyy-MM-dd")))
                .ForMember(x => x.IsDropOut, opt => opt.MapFrom(src => src.IsDropOut))
                .ForMember(x => x.DropOutCode, opt => opt.MapFrom(src => src.DropOutCode))
                .ForMember(x => x.IsTransferred, opt => opt.MapFrom(src => src.IsTransferred))
                .ForMember(x => x.TransferredSchoolName, opt => opt.MapFrom(src => src.TransferredSchoolName))
                .ForMember(x => x.DropOutOtherRemark, opt => opt.MapFrom(src => src.DropOutOtherRemark))
                .ForMember(x => x.IsTransferredIn, opt => opt.MapFrom(src => src.IsTransferredIn))
                .ForMember(x => x.TransferredInSchoolName, opt => opt.MapFrom(src => src.TransferredInSchoolName));

            CreateMap<personEntity, studentTableVM>()
                .ForMember(x => x.firstName, opt => opt.MapFrom(src => src.First_Name))
                .ForMember(x => x.lastName, opt => opt.MapFrom(src => src.Last_Name))
                .ForMember(x => x.middleName, opt => opt.MapFrom(src => src.Middle_Name == null ? "" : src.Middle_Name))
                .ForMember(x => x.idNumber, opt => opt.MapFrom(src => src.ID_Number))
                .ForMember(x => x.gender, opt => opt.MapFrom(src => (src.Gender.ToLower() == "f" ? "Female" : "Male")))
                .ForMember(x => x.birthdate, opt => opt.MapFrom(src => Convert.ToDateTime(src.Birthdate).ToString("yyyy-MM-dd")))
                .ForMember(x => x.address, opt => opt.MapFrom(src => src.Address))
                .ForMember(x => x.emailAddress, opt => opt.MapFrom(src => src.Email_Address))
                .ForMember(x => x.contactNumber, opt => opt.MapFrom(src => src.Contact_Number == null || src.Contact_Number == "undefined" ? "" : src.Contact_Number))
                .ForMember(x => x.educLevelName, opt => opt.MapFrom(src => src.YearSectionEntity.EducationalLevelEntity.Level_Name))
                .ForMember(x => x.yearSecName, opt => opt.MapFrom(src => src.StudentSectionEntity.YearSectionEntity.YearSec_Name))
                .ForMember(x => x.description, opt => opt.MapFrom(src => src.StudentSectionEntity.Description))
                .ForMember(x => x.educLevelId, opt => opt.MapFrom(src => src.StudentSectionEntity.YearSectionEntity.EducationalLevelEntity.Level_ID))
                .ForMember(x => x.yearSecId, opt => opt.MapFrom(src => src.StudentSectionEntity.YearSectionEntity.YearSec_ID))
                .ForMember(x => x.courseId, opt => opt.MapFrom(src => src.CourseEntity.Course_ID))
                .ForMember(x => x.courseName, opt => opt.MapFrom(src => src.CourseEntity.Course_Name))
                .ForMember(x => x.collegeId, opt => opt.MapFrom(src => src.CourseEntity.CollegeEntity.College_ID))
                .ForMember(x => x.collegeName, opt => opt.MapFrom(src => src.CourseEntity.CollegeEntity.College_Name))
                .ForMember(x => x.studSecId, opt => opt.MapFrom(src => src.StudentSectionEntity.StudSec_ID))
                .ForMember(x => x.campusName, opt => opt.MapFrom(src => src.YearSectionEntity.EducationalLevelEntity.CampusEntity.Campus_Name))
                .ForMember(x => x.campusId, opt => opt.MapFrom(src => src.StudentSectionEntity.YearSectionEntity.EducationalLevelEntity.CampusEntity.Campus_ID))
                .ForMember(x => x.emergencyAddress, opt => opt.MapFrom(src => src.EmergencyContactEntity.Address))
                .ForMember(x => x.emergencyFullname, opt => opt.MapFrom(src => src.EmergencyContactEntity.Full_Name))
                .ForMember(x => x.emergencyRelationship, opt => opt.MapFrom(src => src.EmergencyContactEntity.Relationship))
                .ForMember(x => x.emergencyContact, opt => opt.MapFrom(src => src.EmergencyContactEntity.Contact_Number))
                .ForMember(x => x.personId, opt => opt.MapFrom(src => src.Person_ID))
                .ForMember(x => x.dateEnrolled, opt => opt.MapFrom(src => src.DateEnrolled.ToString("yyyy-MM-dd")))
                .ForMember(x => x.isDropOut, opt => opt.MapFrom(src => src.IsDropOut))
                .ForMember(x => x.dropOutCode, opt => opt.MapFrom(src => src.DropOutCode))
                .ForMember(x => x.isTransferred, opt => opt.MapFrom(src => src.IsTransferred))
                .ForMember(x => x.transferredSchoolName, opt => opt.MapFrom(src => src.TransferredSchoolName))
                .ForMember(x => x.dropOutOtherRemark, opt => opt.MapFrom(src => src.DropOutOtherRemark))
                .ForMember(x => x.isTransferredIn, opt => opt.MapFrom(src => src.IsTransferredIn))
                .ForMember(x => x.transferredInSchoolName, opt => opt.MapFrom(src => src.TransferredInSchoolName));

            #endregion

            #region ReportMapping

            CreateMap<cardPagedResult, cardPagedResultVM>();
            CreateMap<cardPagedResultVM, cardPagedResult>();

            CreateMap<personEntity, cardDetailsEntity>()
                .ForMember(x => x.Person_ID, opt => opt.MapFrom(src => src.Person_ID));
            
            CreateMap<personEntity, emergencyContactEntity>()
                .ForMember(x => x.Connected_PersonID, opt => opt.MapFrom(src => src.Person_ID));
            
            //CreateMap<regionEntity, divisionEntity>()
            //    .ForMember(x => x.Region_ID, opt => opt.MapFrom(src => src.ID));


            //CreateMap<CardEntity, CardVM>()
            //    .ForMember(x => x.IDNumber, opt => opt.MapFrom(src => src.IDNumber))
            //    .ForMember(x => x.Name, opt => opt.MapFrom(src => src.Name))
            //    .ForMember(x => x.CardNumber, opt => opt.MapFrom(src => src.CardNumber))
            //    .ForMember(x => x.IssuedDate, opt => opt.MapFrom(src => src.IssuedDate.ToString("yyyy-MM-dd")))
            //    .ForMember(x => x.ExpiryDate, opt => opt.MapFrom(src => src.ExpiryDate.ToString("yyyy-MM-dd")))
            //    .ForMember(x => x.CardStatus, opt => opt.MapFrom(src => src.CardStatus))
            //    .ForMember(x => x.Remarks, opt => opt.MapFrom(src => src.Remarks))
            //    .ForMember(x => x.UpdatedBy, opt => opt.MapFrom(src => src.UpdatedBy))
            //    .ForMember(x => x.DateUpdated, opt => opt.MapFrom(src => src.DateUpdated.ToString("yyyy-MM-dd HH:mm:ss")));

            //CreateMap<tbl_card_details, CardReportVM>()
            //    .ForMember(x => x.CardNumber, opt => opt.MapFrom(src => src.Card_Serial))
            //    .ForMember(x => x.Name, opt => opt.MapFrom(src => src.tbl_person.Last_Name + " " + src.tbl_person.First_Name))
            //    .ForMember(x => x.IDNumber, opt => opt.MapFrom(src => src.tbl_person.ID_Number))
            //    .ForMember(x => x.FirstName, opt => opt.MapFrom(src => src.tbl_person.First_Name))
            //    .ForMember(x => x.LastName, opt => opt.MapFrom(src => src.tbl_person.Last_Name))
            //    .ForMember(x => x.ExpiryDate, opt => opt.MapFrom(src => src.Expiry_date.ToString("yyyy-MM-dd")))
            //    .ForMember(x => x.IssuedDate, opt => opt.MapFrom(src => src.Issued_Date.ToString("yyyy-MM-dd")))
            //    .ForMember(x => x.IssuedDate2, opt => opt.MapFrom(src => ((new DateTimeOffset(new DateTime(
            //                            src.Issued_Date.Year,
            //                            src.Issued_Date.Month,
            //                            src.Issued_Date.Day,
            //                            src.Issued_Date.Hour,
            //                            src.Issued_Date.Minute,
            //                            src.Issued_Date.Second, DateTimeKind.Local)))).ToUnixTimeSeconds()))
            //    .ForMember(x => x.Remarks, opt => opt.MapFrom(src => src.Remarks))
            //    .ForMember(x => x.CardStatus, opt => opt.MapFrom(src => (src.On_Hold == true ? src.IsActive == true ? "ON HOLD" : "NOT VALIDATED" : src.Blocked == true ? src.IsActive == true ? "BLACKLISTED" : "NOT VALIDATED" : src.IsActive == true ? "ACTIVE" : "NOT VALIDATED")));

            CreateMap<cardReportPagedResult, cardReportPagedResultVM>();

            CreateMap<formEntity, formAuditTrailVM>()
                .ForMember(x => x.form_ID, opt => opt.MapFrom(src => src.Form_ID))
                .ForMember(x => x.form_Name, opt => opt.MapFrom(src => src.Form_Name));

            //CreateMap<tbl_daily_logs, TimeAndAttendanceEmployeeVM>()
            //    .ForMember(x => x.ID_Number, opt => opt.MapFrom(src => src.tbl_card_details.tbl_person.ID_Number))
            //    .ForMember(x => x.First_Name, opt => opt.MapFrom(src => src.tbl_card_details.tbl_person.First_Name))
            //    .ForMember(x => x.Log_Date, opt => opt.MapFrom(src => src.Log_Date.ToString("yyyy-MM-dd HH:mm:ss")))
            //    .ForMember(x => x.Log_Date2, opt => opt.MapFrom(src => ((new DateTimeOffset(new DateTime(
            //        src.Log_Date.Year,
            //        src.Log_Date.Month,
            //        src.Log_Date.Day,
            //        src.Log_Date.Hour,
            //        src.Log_Date.Minute,
            //        src.Log_Date.Second, DateTimeKind.Local)))).ToUnixTimeSeconds()))
            //    .ForMember(x => x.Campus_Name, opt => opt.MapFrom(src => src.tbl_terminal.tbl_area.tbl_campus.Campus_Name))
            //    .ForMember(x => x.Department_Name, opt => opt.MapFrom(src => src.tbl_card_details.tbl_person.tblref_position.tblref_department.Department_Name))
            //    .ForMember(x => x.Area_Name, opt => opt.MapFrom(src => src.tbl_terminal.tbl_area.Area_Name))
            //    .ForMember(x => x.Terminal_Name, opt => opt.MapFrom(src => src.tbl_terminal.Terminal_Name))
            //    .ForMember(x => x.Last_Name, opt => opt.MapFrom(src => src.tbl_card_details.tbl_person.Last_Name))
            //    .ForMember(x => x.Status, opt => opt.MapFrom(src => src.Log_Message))
            //    .ForMember(x => x.Name, opt => opt.MapFrom(src => src.tbl_card_details.tbl_person.Last_Name + " " + src.tbl_card_details.tbl_person.First_Name));

            //CreateMap<timeAndAttendanceEmployeePagedResult, timeAndAttendanceEmployeePagedResultVM>();

            //CreateMap<tbl_daily_logs, TimeAndAttendanceStudentVM>()
            //    .ForMember(x => x.ID_Number, opt => opt.MapFrom(src => src.tbl_card_details.tbl_person.ID_Number))
            //    .ForMember(x => x.First_Name, opt => opt.MapFrom(src => src.tbl_card_details.tbl_person.First_Name))
            //    .ForMember(x => x.Log_Date, opt => opt.MapFrom(src => src.Log_Date.ToString("yyyy-MM-dd HH:mm:ss")))
            //    .ForMember(x => x.Log_Date2, opt => opt.MapFrom(src => ((new DateTimeOffset(new DateTime(
            //            src.Log_Date.Year,
            //            src.Log_Date.Month,
            //            src.Log_Date.Day,
            //            src.Log_Date.Hour,
            //            src.Log_Date.Minute,
            //            src.Log_Date.Second, DateTimeKind.Local)))).ToUnixTimeSeconds()))
            //    .ForMember(x => x.Campus_Name, opt => opt.MapFrom(src => src.tbl_terminal.tbl_area.tbl_campus.Campus_Name))
            //    .ForMember(x => x.Area_Name, opt => opt.MapFrom(src => src.tbl_terminal.tbl_area.Area_Name))
            //    .ForMember(x => x.Terminal_Name, opt => opt.MapFrom(src => src.tbl_terminal.Terminal_Name))
            //    .ForMember(x => x.Last_Name, opt => opt.MapFrom(src => src.tbl_card_details.tbl_person.Last_Name))
            //    .ForMember(x => x.Status, opt => opt.MapFrom(src => src.Log_Message))
            //    .ForMember(x => x.Name, opt => opt.MapFrom(src => src.tbl_card_details.tbl_person.Last_Name + " " + src.tbl_card_details.tbl_person.First_Name));

            //CreateMap<timeAndAttendanceStudentPagedResult, timeAndAttendanceStudentPagedResultVM>();

            //CreateMap<tbl_daily_logs, TimeAndAttendanceVisitorVM>()
            //    .ForMember(x => x.ID_Number, opt => opt.MapFrom(src => src.tbl_card_details.tbl_person.ID_Number))
            //    .ForMember(x => x.First_Name, opt => opt.MapFrom(src => src.tbl_card_details.tbl_person.First_Name))
            //    .ForMember(x => x.Log_Date, opt => opt.MapFrom(src => src.Log_Date.ToString("yyyy-MM-dd HH:mm:ss")))
            //    .ForMember(x => x.Log_Date2, opt => opt.MapFrom(src => ((new DateTimeOffset(new DateTime(
            //        src.Log_Date.Year,
            //        src.Log_Date.Month,
            //        src.Log_Date.Day,
            //        src.Log_Date.Hour,
            //        src.Log_Date.Minute,
            //        src.Log_Date.Second, DateTimeKind.Local)))).ToUnixTimeSeconds()))
            //    .ForMember(x => x.Campus_Name, opt => opt.MapFrom(src => src.tbl_terminal.tbl_area.tbl_campus.Campus_Name))
            //    .ForMember(x => x.Area_Name, opt => opt.MapFrom(src => src.tbl_terminal.tbl_area.Area_Name))
            //    .ForMember(x => x.Terminal_Name, opt => opt.MapFrom(src => src.tbl_terminal.Terminal_Name))
            //    .ForMember(x => x.Last_Name, opt => opt.MapFrom(src => src.tbl_card_details.tbl_person.Last_Name))
            //    .ForMember(x => x.Status, opt => opt.MapFrom(src => src.Log_Message))
            //    .ForMember(x => x.Name, opt => opt.MapFrom(src => src.tbl_card_details.tbl_person.Last_Name + " " + src.tbl_card_details.tbl_person.First_Name));

            //CreateMap<timeAndAttendanceVisitorPagedResult, timeAndAttendanceVisitorPagedResultVM>();

            //CreateMap<TimeAttendanceEntity, TimeAttendanceVM>()
            //    .ForMember(x => x.IDNumber, opt => opt.MapFrom(src => src.IDNumber))
            //    .ForMember(x => x.Name, opt => opt.MapFrom(src => src.Name))
            //    .ForMember(x => x.LogDate, opt => opt.MapFrom(src => src.LogDate.ToString("yyyy-MM-dd HH:mm:ss")))
            //    .ForMember(x => x.CampusName, opt => opt.MapFrom(src => src.CampusName))
            //    .ForMember(x => x.AreaName, opt => opt.MapFrom(src => src.AreaName))
            //    .ForMember(x => x.TerminalName, opt => opt.MapFrom(src => src.TerminalName))
            //    .ForMember(x => x.Status, opt => opt.MapFrom(src => src.Status));

            //CreateMap<TimeAttendanceVisitorEntity, TimeAttendanceVisitorVM>()
            //    .ForMember(x => x.IDNumber, opt => opt.MapFrom(src => src.IDNumber))
            //    .ForMember(x => x.Name, opt => opt.MapFrom(src => src.Name))
            //    .ForMember(x => x.LogDate, opt => opt.MapFrom(src => src.LogDate.ToString("yyyy-MM-dd HH:mm:ss")))
            //    .ForMember(x => x.CampusName, opt => opt.MapFrom(src => src.CampusName))
            //    .ForMember(x => x.AreaName, opt => opt.MapFrom(src => src.AreaName))
            //    .ForMember(x => x.TerminalName, opt => opt.MapFrom(src => src.TerminalName))
            //    .ForMember(x => x.Status, opt => opt.MapFrom(src => src.Status));

            //CreateMap<AlarmEntity, AlarmVM>()
            //    .ForMember(x => x.IDNumber, opt => opt.MapFrom(src => src.IDNumber))
            //    .ForMember(x => x.Name, opt => opt.MapFrom(src => src.Name))
            //    .ForMember(x => x.LogDate, opt => opt.MapFrom(src => src.LogDate.ToString("yyyy-MM-dd HH:mm:ss")))
            //    .ForMember(x => x.CampusName, opt => opt.MapFrom(src => src.CampusName))
            //    .ForMember(x => x.AreaName, opt => opt.MapFrom(src => src.AreaName))
            //    .ForMember(x => x.TerminalName, opt => opt.MapFrom(src => src.TerminalName))
            //    .ForMember(x => x.Status, opt => opt.MapFrom(src => src.Status));

            //CreateMap<AlarmEmployeeEntity, AlarmEmployeeVM>()
            //   .ForMember(x => x.IDNumber, opt => opt.MapFrom(src => src.IDNumber))
            //   .ForMember(x => x.Name, opt => opt.MapFrom(src => src.Name))
            //   .ForMember(x => x.LogDate, opt => opt.MapFrom(src => src.LogDate.ToString("yyyy-MM-dd HH:mm:ss")))
            //   .ForMember(x => x.CampusName, opt => opt.MapFrom(src => src.CampusName))
            //   .ForMember(x => x.AreaName, opt => opt.MapFrom(src => src.AreaName))
            //   .ForMember(x => x.TerminalName, opt => opt.MapFrom(src => src.TerminalName))
            //   .ForMember(x => x.Status, opt => opt.MapFrom(src => src.Status));

            //CreateMap<AlarmVisitorEntity, AlarmVisitorVM>()
            //    .ForMember(x => x.IDNumber, opt => opt.MapFrom(src => src.IDNumber))
            //    .ForMember(x => x.Name, opt => opt.MapFrom(src => src.Name))
            //    .ForMember(x => x.LogDate, opt => opt.MapFrom(src => src.LogDate.ToString("yyyy-MM-dd HH:mm:ss")))
            //    .ForMember(x => x.CampusName, opt => opt.MapFrom(src => src.CampusName))
            //    .ForMember(x => x.AreaName, opt => opt.MapFrom(src => src.AreaName))
            //    .ForMember(x => x.TerminalName, opt => opt.MapFrom(src => src.TerminalName))
            //    .ForMember(x => x.Status, opt => opt.MapFrom(src => src.Status));

            //CreateMap<tbl_daily_logs, AlarmEmployeeVM>()
            //    .ForMember(x => x.ID_Number, opt => opt.MapFrom(src => src.tbl_card_details.tbl_person.ID_Number))
            //    .ForMember(x => x.First_Name, opt => opt.MapFrom(src => src.tbl_card_details.tbl_person.First_Name))
            //    .ForMember(x => x.Log_Date, opt => opt.MapFrom(src => src.Log_Date.ToString("yyyy-MM-dd HH:mm:ss")))
            //    .ForMember(x => x.Campus_Name, opt => opt.MapFrom(src => src.tbl_terminal.tbl_area.tbl_campus.Campus_Name))
            //    .ForMember(x => x.Area_Name, opt => opt.MapFrom(src => src.tbl_terminal.tbl_area.Area_Name))
            //    .ForMember(x => x.Terminal_Name, opt => opt.MapFrom(src => src.tbl_terminal.Terminal_Name))
            //    .ForMember(x => x.Last_Name, opt => opt.MapFrom(src => src.tbl_card_details.tbl_person.Last_Name))
            //    .ForMember(x => x.Status, opt => opt.MapFrom(src => src.Log_Message))
            //    .ForMember(x => x.Name, opt => opt.MapFrom(src => src.tbl_card_details.tbl_person.Last_Name + " " + src.tbl_card_details.tbl_person.First_Name));

            //CreateMap<AlarmEmployeePagedResult, AlarmEmployeePagedResultVM>();

            //CreateMap<tbl_daily_logs, AlarmStudentVM>()
            //    .ForMember(x => x.ID_Number, opt => opt.MapFrom(src => src.tbl_card_details.tbl_person.ID_Number))
            //    .ForMember(x => x.First_Name, opt => opt.MapFrom(src => src.tbl_card_details.tbl_person.First_Name))
            //    .ForMember(x => x.Log_Date, opt => opt.MapFrom(src => src.Log_Date.ToString("yyyy-MM-dd HH:mm:ss")))
            //    .ForMember(x => x.Campus_Name, opt => opt.MapFrom(src => src.tbl_terminal.tbl_area.tbl_campus.Campus_Name))
            //    .ForMember(x => x.Area_Name, opt => opt.MapFrom(src => src.tbl_terminal.tbl_area.Area_Name))
            //    .ForMember(x => x.Terminal_Name, opt => opt.MapFrom(src => src.tbl_terminal.Terminal_Name))
            //    .ForMember(x => x.Last_Name, opt => opt.MapFrom(src => src.tbl_card_details.tbl_person.Last_Name))
            //    .ForMember(x => x.Status, opt => opt.MapFrom(src => src.Log_Message))
            //    .ForMember(x => x.Name, opt => opt.MapFrom(src => src.tbl_card_details.tbl_person.Last_Name + " " + src.tbl_card_details.tbl_person.First_Name));

            //CreateMap<AlarmStudentPagedResult, AlarmStudentPagedResultVM>();

            //CreateMap<tbl_daily_logs, AlarmVisitorVM>()
            //    .ForMember(x => x.ID_Number, opt => opt.MapFrom(src => src.tbl_card_details.tbl_person.ID_Number))
            //    .ForMember(x => x.First_Name, opt => opt.MapFrom(src => src.tbl_card_details.tbl_person.First_Name))
            //    .ForMember(x => x.Log_Date, opt => opt.MapFrom(src => src.Log_Date.ToString("yyyy-MM-dd HH:mm:ss")))
            //    .ForMember(x => x.Campus_Name, opt => opt.MapFrom(src => src.tbl_terminal.tbl_area.tbl_campus.Campus_Name))
            //    .ForMember(x => x.Area_Name, opt => opt.MapFrom(src => src.tbl_terminal.tbl_area.Area_Name))
            //    .ForMember(x => x.Terminal_Name, opt => opt.MapFrom(src => src.tbl_terminal.Terminal_Name))
            //    .ForMember(x => x.Last_Name, opt => opt.MapFrom(src => src.tbl_card_details.tbl_person.Last_Name))
            //    .ForMember(x => x.Status, opt => opt.MapFrom(src => src.Log_Message))
            //    .ForMember(x => x.Name, opt => opt.MapFrom(src => src.tbl_card_details.tbl_person.Last_Name + " " + src.tbl_card_details.tbl_person.First_Name));

            //CreateMap<AlarmVisitorPagedResult, AlarmVisitorPagedResultVM>();

            //CreateMap<tbl_audit_trail, AuditTrailReportVM>()
            //    .ForMember(x => x.ID, opt => opt.MapFrom(src => src.ID))
            //    .ForMember(x => x.Form_ID, opt => opt.MapFrom(src => src.Form_ID))
            //    .ForMember(x => x.Form_Name, opt => opt.MapFrom(src => src.tbl_form.Form_Name))
            //    .ForMember(x => x.User_ID, opt => opt.MapFrom(src => src.User_ID))
            //    .ForMember(x => x.User_Name, opt => opt.MapFrom(src => src.tbl_user.User_Name))
            //    .ForMember(x => x.Action, opt => opt.MapFrom(src => src.Action))
            //    .ForMember(x => x.Date, opt => opt.MapFrom(src => src.Date.ToString("yyyy-MM-dd HH:mm:ss")))
            //    .ForMember(x => x.Date2, opt => opt.MapFrom(src => ((new DateTimeOffset(new DateTime(
            //            src.Date.Year,
            //            src.Date.Month,
            //            src.Date.Day,
            //            src.Date.Hour,
            //            src.Date.Minute,
            //            src.Date.Second, DateTimeKind.Local)))).ToUnixTimeSeconds()));

            CreateMap<auditTrailPagedResult, auditTrailPagedResultVM>();
            CreateMap<timeAndAttendanceStudentPagedResult, timeAndAttendanceStudentPagedResultVM>();

            ////CreateMap<Visitor_Report, visitorVM>()
            //// .ForMember(x => x.da, opt => opt.MapFrom(src => src.Date.ToString("MM/dd/yyyy hh:ss")));

            //CreateMap<VisitorReportEntity, VisitorReportVM>()
            //    .ForMember(x => x.VisitorInformationID, opt => opt.MapFrom(src => src.VisitorInformationID))
            //    .ForMember(x => x.TrackingCode, opt => opt.MapFrom(src => src.TrackingCode))
            //    .ForMember(x => x.VisitorName, opt => opt.MapFrom(src => src.VisitorName))
            //    .ForMember(x => x.Gender, opt => opt.MapFrom(src => src.Gender))
            //    .ForMember(x => x.PresentedID, opt => opt.MapFrom(src => src.PresentedID))
            //    .ForMember(x => x.PresentedIDType, opt => opt.MapFrom(src => src.PresentedIDType))
            //    .ForMember(x => x.ScheduleDate, opt => opt.MapFrom(src => Convert.ToDateTime(src.ScheduleDate).ToString("yyyy-MM-dd")))
            //    .ForMember(x => x.DateTimeTagged, opt => opt.MapFrom(src => Convert.ToDateTime(src.DateTimeTagged).ToString("yyyy-MM-dd HH:mm:ss")))
            //    .ForMember(x => x.DateTimeSurrendered, opt => opt.MapFrom(src => Convert.ToDateTime(src.DateTimeSurrendered).ToString("yyyy-MM-dd HH:mm:ss") == "0001-01-01 00:00:00" ? string.Empty : Convert.ToDateTime(src.DateTimeSurrendered).ToString("yyyy-MM-dd HH:mm:ss")))
            //    .ForMember(x => x.VisitorCard, opt => opt.MapFrom(src => src.VisitorCard))
            //    .ForMember(x => x.VisitedPerson, opt => opt.MapFrom(src => src.VisitedPerson))
            //    .ForMember(x => x.ManualOutRemarks, opt => opt.MapFrom(src => src.ManualOutRemarks))
            //    .ForMember(x => x.CardDetailsID, opt => opt.MapFrom(src => src.CardDetailsID));

            //CreateMap<visitorReportPagedResult, visitorReportPagedResultVM>();

            //CreateMap<VisitorInformation, VisitorInformationVM>()
            //    .ForMember(x => x.trackingCode, opt => opt.MapFrom(src => src.trackingCode))
            //    .ForMember(x => x.visitorName, opt => opt.MapFrom(src => src.Visitor.lastName + " " + src.Visitor.firstName))
            //    .ForMember(x => x.Gender, opt => opt.MapFrom(src => src.Visitor.Gender))
            //    .ForMember(x => x.presentedIdNumber, opt => opt.MapFrom(src => src.presentedIdNumber))
            //    .ForMember(x => x.presentedIdType, opt => opt.MapFrom(src => src.presentedIdType))
            //    .ForMember(x => x.scheduleDate, opt => opt.MapFrom(src => Convert.ToDateTime(src.scheduleDate).ToString("yyyy-MM-dd")))
            //    .ForMember(x => x.dateTimeTagged, opt => opt.MapFrom(src => Convert.ToDateTime(src.dateTimeTagged).ToString("yyyy-MM-dd HH:mm:ss")))
            //    .ForMember(x => x.dateTimeSurrendered, opt => opt.MapFrom(src => Convert.ToDateTime(src.dateTimeSurrendered).ToString("yyyy-MM-dd HH:mm:ss") == "0001-01-01 00:00:00" ? string.Empty : Convert.ToDateTime(src.dateTimeSurrendered).ToString("yyyy-MM-dd HH:mm:ss")))
            //    .ForMember(x => x.visitorCard, opt => opt.MapFrom(src => src.cardDetails.tbl_person.ID_Number))
            //    .ForMember(x => x.visitedPerson, opt => opt.MapFrom(src => src.visitedPerson.Last_Name + " " + src.visitedPerson.First_Name))
            //    .ForMember(x => x.manualOut, opt => opt.MapFrom(src => src.manualOut))
            //    .ForMember(x => x.remarks, opt => opt.MapFrom(src => src.remarks))
            //    .ForMember(x => x.NameOfEmployer, opt => opt.MapFrom(src => src.Visitor.Name_Of_Employer));

            //CreateMap<Card_Employee, CardEmployeeVM>()
            // .ForMember(x => x.Status, opt => opt.MapFrom(src => src.IsActive == true ? "Active" : "Inactive"))
            // .ForMember(x => x.Issued_Date, opt => opt.MapFrom(src => src.Issued_Date.ToString("MM/dd/yyyy")))
            // .ForMember(x => x.Last_Updated, opt => opt.MapFrom(src => src.Last_Updated.ToString("MM/dd/yyyy")));

            //CreateMap<Card_Student, CardStudentVM>()
            // .ForMember(x => x.Status, opt => opt.MapFrom(src => src.IsActive == true ? "Active" : "Inactive"))
            // .ForMember(x => x.Issued_Date, opt => opt.MapFrom(src => src.Issued_Date.ToString("MM/dd/yyyy")))
            // .ForMember(x => x.Last_Updated, opt => opt.MapFrom(src => src.Last_Updated.ToString("MM/dd/yyyy")));

            //CreateMap<Total_Hours_Employee, TotalHoursOfEmployeeVM>()
            //    .ForMember(x => x.ID_Number, opt => opt.MapFrom(src => src.ID_Number))
            //    .ForMember(x => x.Full_Name, opt => opt.MapFrom(src => src.Full_Name))
            //    .ForMember(x => x.Department_Name, opt => opt.MapFrom(src => src.Department_Name))
            //    .ForMember(x => x.Employee_Type, opt => opt.MapFrom(src => src.Employee_Type))
            //    .ForMember(x => x.Total_Hours, opt => opt.MapFrom(src => src.Total_Hours));

            #endregion

            #region FormMapping

            CreateMap<formEntity, formVM>()
             .ForMember(x => x.Form_ID, opt => opt.MapFrom(src => src.Form_ID))
             .ForMember(x => x.Form_Name, opt => opt.MapFrom(src => src.Form_Name));

            #endregion
            
            #region VisitorMapping

            //CreateMap<Visitor, visitorVM>()
            // .ForMember(x => x.Visitor_ID, opt => opt.MapFrom(src => src.id))
            // .ForMember(x => x.FirstName, opt => opt.MapFrom(src => src.firstName))
            // .ForMember(x => x.MiddleName, opt => opt.MapFrom(src => src.middleName))
            // .ForMember(x => x.LastName, opt => opt.MapFrom(src => src.lastName))
            // .ForMember(x => x.Address, opt => opt.MapFrom(src => src.address))
            // .ForMember(x => x.IDNumber, opt => opt.MapFrom(src => src.VisitorInformation.presentedIdNumber))
            // .ForMember(x => x.Campus_Name, opt => opt.MapFrom(src => src.tbl_campus.Campus_Name))
            // .ForMember(x => x.Campus_ID, opt => opt.MapFrom(src => src.Campus_ID));

            CreateMap<personEntity, personVisitorVM>()
                .ForMember(x => x.status, opt => opt.MapFrom(src => src.IsActive == true ? "Active" : "Inactive"))
                .ForMember(x => x.personId, opt => opt.MapFrom(src => src.Person_ID))
                .ForMember(x => x.idNumber, opt => opt.MapFrom(src => src.ID_Number))
                .ForMember(x => x.firstName, opt => opt.MapFrom(src => src.First_Name))
                .ForMember(x => x.middleName, opt => opt.MapFrom(src => (src.Middle_Name == null || src.Middle_Name == "undefined" ? string.Empty : src.Middle_Name)))
                .ForMember(x => x.lastName, opt => opt.MapFrom(src => src.Last_Name))
                .ForMember(x => x.gender, opt => opt.MapFrom(src => (src.Gender)))
                .ForMember(x => x.birthdate, opt => opt.MapFrom(src => Convert.ToDateTime(src.Birthdate).ToString("yyyy-MM-dd")))
                .ForMember(x => x.address, opt => opt.MapFrom(src => src.Address))
                .ForMember(x => x.emailAddress, opt => opt.MapFrom(src => src.Email_Address))

                .ForMember(x => x.contactNumber, opt => opt.MapFrom(src => src.Contact_Number));


            CreateMap<personEntity, personVisitorTableVM>()
             .ForMember(x => x.status, opt => opt.MapFrom(src => src.IsActive == true ? "Active" : "Inactive"))
             .ForMember(x => x.personId, opt => opt.MapFrom(src => src.Person_ID))
             .ForMember(x => x.idNumber, opt => opt.MapFrom(src => src.ID_Number))
             .ForMember(x => x.firstName, opt => opt.MapFrom(src => src.First_Name))
             .ForMember(x => x.middleName, opt => opt.MapFrom(src => (src.Middle_Name == null || src.Middle_Name == "undefined" ? string.Empty : src.Middle_Name)))
             .ForMember(x => x.lastName, opt => opt.MapFrom(src => src.Last_Name))
             .ForMember(x => x.gender, opt => opt.MapFrom(src => (src.Gender.ToLower() == "m" ? "Male" : "Female")))
             .ForMember(x => x.birthdate, opt => opt.MapFrom(src => Convert.ToDateTime(src.Birthdate).ToString("yyyy-MM-dd")))
             .ForMember(x => x.address, opt => opt.MapFrom(src => src.Address))
             .ForMember(x => x.emailAddress, opt => opt.MapFrom(src => src.Email_Address == null || src.Email_Address == "undefined" ? "" : src.Email_Address))

             .ForMember(x => x.contactNumber, opt => opt.MapFrom(src => src.Contact_Number));

            CreateMap<visitorPersonBatchUploadHeaders, visitorPersonBatchUploadVM>()
                .ForMember(x => x.IDNumber, opt => opt.MapFrom(src => src.IDNumber))
                .ForMember(x => x.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(x => x.MiddleName, opt => opt.MapFrom(src => src.MiddleName))
                .ForMember(x => x.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(x => x.Gender, opt => opt.MapFrom(src => src.Gender))
                .ForMember(x => x.BirthDate, opt => opt.MapFrom(src => src.BirthDate))
                .ForMember(x => x.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(x => x.EmailAddress, opt => opt.MapFrom(src => src.EmailAddress == null || src.EmailAddress == "undefined" ? "" : src.EmailAddress))
                .ForMember(x => x.ContactNumber, opt => opt.MapFrom(src => src.ContactNumber));

            CreateMap<personVisitorVM, personEntity>()
                .ForMember(x => x.Person_ID, opt => opt.MapFrom(src => src.personId))
                .ForMember(x => x.ID_Number, opt => opt.MapFrom(src => src.idNumber))
                .ForMember(x => x.First_Name, opt => opt.MapFrom(src => src.firstName))
                .ForMember(x => x.Middle_Name, opt => opt.MapFrom(src => src.middleName == null || src.middleName == "undefined" ? "" : src.middleName))
                .ForMember(x => x.Last_Name, opt => opt.MapFrom(src => src.lastName))
                .ForMember(x => x.Gender, opt => opt.MapFrom(src => (src.gender.ToLower() == "female" ? "F" : "M")))
                .ForMember(x => x.Birthdate, opt => opt.MapFrom(src => (src.birthdate == null ? DateTime.Now : Convert.ToDateTime(src.birthdate))))
                .ForMember(x => x.Address, opt => opt.MapFrom(src => src.address))
                .ForMember(x => x.Email_Address, opt => opt.MapFrom(src => (src.emailAddress == null || src.emailAddress == "undefined" ? string.Empty : src.emailAddress)))
                .ForMember(x => x.Contact_Number, opt => opt.MapFrom(src => (src.contactNumber == null ? string.Empty : src.contactNumber)));

            #endregion

            #region FetcherMapping

            CreateMap<personEntity, fetcherVM>()
                .ForMember(x => x.personId, opt => opt.MapFrom(src => src.Person_ID))
                .ForMember(x => x.idNumber, opt => opt.MapFrom(src => src.ID_Number))
                .ForMember(x => x.firstName, opt => opt.MapFrom(src => src.First_Name))
                .ForMember(x => x.middleName, opt => opt.MapFrom(src => (src.Middle_Name == null || src.Middle_Name == "undefined" ? string.Empty : src.Middle_Name)))
                .ForMember(x => x.lastName, opt => opt.MapFrom(src => src.Last_Name))
                .ForMember(x => x.gender, opt => opt.MapFrom(src => (src.Gender)))
                .ForMember(x => x.birthdate, opt => opt.MapFrom(src => Convert.ToDateTime(src.Birthdate).ToString("yyyy-MM-dd")))
                .ForMember(x => x.address, opt => opt.MapFrom(src => src.Address))
                .ForMember(x => x.emailAddress, opt => opt.MapFrom(src => src.Email_Address))
                .ForMember(x => x.contactNumber, opt => opt.MapFrom(src => src.Contact_Number))
                .ForMember(x => x.fetcherRelationship, opt => opt.MapFrom(src => (src.Fetcher_Relationship == "undefined" ? string.Empty : src.Fetcher_Relationship) ));
            
            CreateMap<fetcherVM, personEntity>()
                .ForMember(x => x.Person_ID, opt => opt.MapFrom(src => src.personId))
                .ForMember(x => x.ID_Number, opt => opt.MapFrom(src => src.idNumber))
                .ForMember(x => x.First_Name, opt => opt.MapFrom(src => src.firstName))
                .ForMember(x => x.Middle_Name, opt => opt.MapFrom(src => (src.middleName == null || src.middleName == "undefined" ? string.Empty : src.middleName)))
                .ForMember(x => x.Last_Name, opt => opt.MapFrom(src => src.lastName))
                .ForMember(x => x.Gender, opt => opt.MapFrom(src => (src.gender.ToLower() == "female" ? "F" : "M")))
                .ForMember(x => x.Birthdate, opt => opt.MapFrom(src => (src.birthdate == null ? DateTime.Now : Convert.ToDateTime(src.birthdate))))
                .ForMember(x => x.Address, opt => opt.MapFrom(src => src.address))
                .ForMember(x => x.Email_Address, opt => opt.MapFrom(src => (src.emailAddress == null ? string.Empty : src.emailAddress)))
                .ForMember(x => x.Contact_Number, opt => opt.MapFrom(src => (src.contactNumber == null ? string.Empty : src.contactNumber)))
                .ForMember(x => x.Fetcher_Relationship, opt => opt.MapFrom(src => (src.fetcherRelationship == null || src.fetcherRelationship == "undefined" ? string.Empty : src.fetcherRelationship)));
           
            CreateMap<personEntity, personFetcherVM>()
                .ForMember(x => x.status, opt => opt.MapFrom(src => src.IsActive == true ? "Active" : "Inactive"))
                .ForMember(x => x.personId, opt => opt.MapFrom(src => src.Person_ID))
                .ForMember(x => x.idNumber, opt => opt.MapFrom(src => src.ID_Number))
                .ForMember(x => x.firstName, opt => opt.MapFrom(src => src.First_Name))
                .ForMember(x => x.middleName, opt => opt.MapFrom(src => (src.Middle_Name == null || src.Middle_Name == "undefined" ? string.Empty : src.Middle_Name)))
                .ForMember(x => x.lastName, opt => opt.MapFrom(src => src.Last_Name))
                .ForMember(x => x.gender, opt => opt.MapFrom(src => (src.Gender)))
                .ForMember(x => x.birthdate, opt => opt.MapFrom(src => Convert.ToDateTime(src.Birthdate).ToString("yyyy-MM-dd")))
                .ForMember(x => x.address, opt => opt.MapFrom(src => src.Address))
                .ForMember(x => x.emailAddress, opt => opt.MapFrom(src => (src.Email_Address == null || src.Email_Address == "undefined" ? string.Empty : src.Email_Address)))
                .ForMember(x => x.contactNumber, opt => opt.MapFrom(src => src.Contact_Number))
                .ForMember(x => x.fetcherRelationship, opt => opt.MapFrom(src => src.Fetcher_Relationship))
                .ForMember(x => x.campusName, opt => opt.MapFrom(src => src.CampusEntity.Campus_Name))
                .ForMember(x => x.campusId, opt => opt.MapFrom(src => src.Campus_ID));

            CreateMap<personEntity, personFetcherTableVM>()
             .ForMember(x => x.status, opt => opt.MapFrom(src => src.IsActive == true ? "Active" : "Inactive"))
             .ForMember(x => x.personId, opt => opt.MapFrom(src => src.Person_ID))
             .ForMember(x => x.idNumber, opt => opt.MapFrom(src => src.ID_Number))
             .ForMember(x => x.firstName, opt => opt.MapFrom(src => src.First_Name))
             .ForMember(x => x.middleName, opt => opt.MapFrom(src => (src.Middle_Name == null || src.Middle_Name == "undefined" ? string.Empty : src.Middle_Name)))
             .ForMember(x => x.lastName, opt => opt.MapFrom(src => src.Last_Name))
             .ForMember(x => x.gender, opt => opt.MapFrom(src => (src.Gender.ToLower() == "m" ? "Male" : "Female")))
             .ForMember(x => x.birthdate, opt => opt.MapFrom(src => Convert.ToDateTime(src.Birthdate).ToString("yyyy-MM-dd")))
             .ForMember(x => x.address, opt => opt.MapFrom(src => src.Address))
             .ForMember(x => x.emailAddress, opt => opt.MapFrom(src => (src.Email_Address == null || src.Email_Address == "undefined" ? string.Empty : src.Email_Address)))
             .ForMember(x => x.contactNumber, opt => opt.MapFrom(src => src.Contact_Number))
             .ForMember(x => x.fetcherRelationship, opt => opt.MapFrom(src => src.Fetcher_Relationship))
             .ForMember(x => x.campusId, opt => opt.MapFrom(src => src.Campus_ID))
             .ForMember(x => x.campusName, opt => opt.MapFrom(src => src.CampusEntity.Campus_Name));

            CreateMap<personFetcherVM, personEntity>()
                .ForMember(x => x.Person_ID, opt => opt.MapFrom(src => src.personId))
                .ForMember(x => x.ID_Number, opt => opt.MapFrom(src => src.idNumber))
                .ForMember(x => x.First_Name, opt => opt.MapFrom(src => src.firstName))
                .ForMember(x => x.Middle_Name, opt => opt.MapFrom(src => (src.middleName == null || src.middleName == "undefined" ? string.Empty : src.middleName)))
                .ForMember(x => x.Last_Name, opt => opt.MapFrom(src => src.lastName))
                .ForMember(x => x.Gender, opt => opt.MapFrom(src => (src.gender.ToLower() == "female" ? "F" : "M")))
                .ForMember(x => x.Birthdate, opt => opt.MapFrom(src => (src.birthdate == null ? DateTime.Now : Convert.ToDateTime(src.birthdate))))
                .ForMember(x => x.Address, opt => opt.MapFrom(src => src.address))
                .ForMember(x => x.Email_Address, opt => opt.MapFrom(src => (src.emailAddress == null || src.emailAddress == "undefined" ? string.Empty : src.emailAddress)))
                .ForMember(x => x.Contact_Number, opt => opt.MapFrom(src => (src.contactNumber == null ? string.Empty : src.contactNumber)))
                .ForMember(x => x.Fetcher_Relationship, opt => opt.MapFrom(src => src.fetcherRelationship));

            CreateMap<emergencyLogoutEntity, emergencyLogoutVM>()
                .ForMember(x => x.status, opt => opt.MapFrom(src => src.IsActive == true ? "Active" : "Inactive"))
                .ForMember(x => x.remarks, opt => opt.MapFrom(src => src.Remarks))
                .ForMember(x => x.effectivityDate, opt => opt.MapFrom(src => src.EffectivityDate.ToString("yyyy-MM-dd")))
                .ForMember(x => x.studentName, opt => opt.MapFrom(src => (src.PersonEntity.Last_Name + ", " + src.PersonEntity.First_Name)))
                .ForMember(x => x.idNumber, opt => opt.MapFrom(src => src.PersonEntity.ID_Number))
                .ForMember(x => x.personId, opt => opt.MapFrom(src => src.Person_ID))
                .ForMember(x => x.emergencyLogoutId, opt => opt.MapFrom(src => src.Emergency_logout_ID));

            CreateMap<emergencyLogoutVM, emergencyLogoutEntity>()
                .ForMember(x => x.Emergency_logout_ID, opt => opt.MapFrom(src => src.emergencyLogoutId))
                .ForMember(x => x.Person_ID, opt => opt.MapFrom(src => src.personId))
                .ForMember(x => x.Remarks, opt => opt.MapFrom(src => src.remarks))
                .ForMember(x => x.EffectivityDate, opt => opt.MapFrom(src => src.effectivityDate));

            CreateMap<personEntity, emergencyLogoutStudentsVM>()
                .ForMember(x => x.studentName, opt => opt.MapFrom(src => (src.ID_Number + " - " + src.Last_Name + ", " + src.First_Name)))
                .ForMember(x => x.personId, opt => opt.MapFrom(src => src.Person_ID));

            CreateMap<personEntity, reportPersonListVM>()
                .ForMember(x => x.personName, opt => opt.MapFrom(src => (src.ID_Number + " - " + src.Last_Name + ", " + src.First_Name)))
                .ForMember(x => x.personId, opt => opt.MapFrom(src => src.Person_ID));

            #endregion

            #region OtherAccessMapping

            CreateMap<personEntity, otherAccessVM>()
                .ForMember(x => x.personId, opt => opt.MapFrom(src => src.Person_ID))
                .ForMember(x => x.idNumber, opt => opt.MapFrom(src => src.ID_Number))
                .ForMember(x => x.firstName, opt => opt.MapFrom(src => src.First_Name))
                .ForMember(x => x.middleName, opt => opt.MapFrom(src => (src.Middle_Name == null || src.Middle_Name == "undefined" ? string.Empty : src.Middle_Name)))
                .ForMember(x => x.lastName, opt => opt.MapFrom(src => src.Last_Name))
                .ForMember(x => x.gender, opt => opt.MapFrom(src => (src.Gender)))
                .ForMember(x => x.birthdate, opt => opt.MapFrom(src => Convert.ToDateTime(src.Birthdate).ToString("yyyy-MM-dd")))
                .ForMember(x => x.address, opt => opt.MapFrom(src => src.Address))
                .ForMember(x => x.emailAddress, opt => opt.MapFrom(src => (src.Email_Address == null || src.Email_Address == "undefined" ? string.Empty : src.Email_Address)))
                .ForMember(x => x.contactNumber, opt => opt.MapFrom(src => src.Contact_Number))
                .ForMember(x => x.telephoneNumber, opt => opt.MapFrom(src => (src.Telephone_Number == null || src.Telephone_Number == "undefined" ? string.Empty : src.Telephone_Number)))
                .ForMember(x => x.departmentId, opt => opt.MapFrom(src => src.PositionEntity.DepartmentEntity.Department_ID))
                .ForMember(x => x.departmentName, opt => opt.MapFrom(src => src.PositionEntity.DepartmentEntity.Department_Name))
                .ForMember(x => x.officeId, opt => opt.MapFrom(src => src.OfficeEntity.Office_ID))
                .ForMember(x => x.officeName, opt => opt.MapFrom(src => src.OfficeEntity.Office_Name))
                .ForMember(x => x.positionId, opt => opt.MapFrom(src => src.PositionEntity.Position_ID))
                .ForMember(x => x.positionName, opt => opt.MapFrom(src => src.PositionEntity.Position_Name))
                .ForMember(x => x.campusId, opt => opt.MapFrom(src => src.PositionEntity.DepartmentEntity.CampusEntity.Campus_ID))
                .ForMember(x => x.campusName, opt => opt.MapFrom(src => src.PositionEntity.DepartmentEntity.CampusEntity.Campus_Name))
                .ForMember(x => x.emergencyAddress, opt => opt.MapFrom(src => src.EmergencyContactEntity.Address))
                .ForMember(x => x.emergencyFullname, opt => opt.MapFrom(src => src.EmergencyContactEntity.Full_Name))
                .ForMember(x => x.emergencyRelationship, opt => opt.MapFrom(src => src.EmergencyContactEntity.Relationship))
                .ForMember(x => x.emergencyContact, opt => opt.MapFrom(src => src.EmergencyContactEntity.Contact_Number));

            CreateMap<otherAccessVM, personEntity>()
                .ForMember(x => x.Person_ID, opt => opt.MapFrom(src => src.personId))
                .ForMember(x => x.ID_Number, opt => opt.MapFrom(src => src.idNumber))
                .ForMember(x => x.First_Name, opt => opt.MapFrom(src => src.firstName))
                .ForMember(x => x.Middle_Name, opt => opt.MapFrom(src => (src.middleName == null || src.emailAddress.ToLower() == "undefined" ? string.Empty : src.middleName)))
                .ForMember(x => x.Last_Name, opt => opt.MapFrom(src => src.lastName))
                .ForMember(x => x.Gender, opt => opt.MapFrom(src => (src.gender.ToLower() == "female" ? "F" : "M")))
                .ForMember(x => x.Birthdate, opt => opt.MapFrom(src => (src.birthdate == null ? DateTime.Now : Convert.ToDateTime(src.birthdate))))
                .ForMember(x => x.Address, opt => opt.MapFrom(src => src.address))
                .ForMember(x => x.Email_Address, opt => opt.MapFrom(src => (src.emailAddress == null || src.emailAddress.ToLower() == "undefined" ? string.Empty : src.emailAddress)))
                .ForMember(x => x.Contact_Number, opt => opt.MapFrom(src => (src.contactNumber == null ? string.Empty : src.contactNumber)))
                .ForMember(x => x.Telephone_Number, opt => opt.MapFrom(src => (src.telephoneNumber == null ? string.Empty : src.telephoneNumber)))
                .ForMember(x => x.Department_ID, opt => opt.MapFrom(src => src.departmentId))
                .ForMember(x => x.Office_ID, opt => opt.MapFrom(src => src.officeId))
                .ForMember(x => x.Position_ID, opt => opt.MapFrom(src => src.positionId))
                .ForMember(x => x.Campus_ID, opt => opt.MapFrom(src => src.campusId));

            CreateMap<otherAccessVM, emergencyContactEntity>()
                .ForMember(x => x.Contact_Number, opt => opt.MapFrom(src => src.emergencyContact))
                .ForMember(x => x.Address, opt => opt.MapFrom(src => src.emergencyAddress))
                .ForMember(x => x.Full_Name, opt => opt.MapFrom(src => src.emergencyFullname))
                .ForMember(x => x.Relationship, opt => opt.MapFrom(src => src.emergencyRelationship));

            CreateMap<personEntity, personOtherAccessVM>()
                .ForMember(x => x.status, opt => opt.MapFrom(src => src.IsActive == true ? "Active" : "Inactive"))
                .ForMember(x => x.personId, opt => opt.MapFrom(src => src.Person_ID))
                .ForMember(x => x.idNumber, opt => opt.MapFrom(src => src.ID_Number))
                .ForMember(x => x.firstName, opt => opt.MapFrom(src => src.First_Name))
                .ForMember(x => x.middleName, opt => opt.MapFrom(src => (src.Middle_Name == null || src.Middle_Name == "undefined" ? string.Empty : src.Middle_Name)))
                .ForMember(x => x.lastName, opt => opt.MapFrom(src => src.Last_Name))
                .ForMember(x => x.gender, opt => opt.MapFrom(src => (src.Gender)))
                .ForMember(x => x.birthdate, opt => opt.MapFrom(src => Convert.ToDateTime(src.Birthdate).ToString("yyyy-MM-dd")))
                .ForMember(x => x.address, opt => opt.MapFrom(src => src.Address))
                .ForMember(x => x.emailAddress, opt => opt.MapFrom(src => (src.Email_Address == null || src.Email_Address == "undefined" ? string.Empty : src.Email_Address)))
                .ForMember(x => x.contactNumber, opt => opt.MapFrom(src => src.Contact_Number))
                .ForMember(x => x.telephoneNumber, opt => opt.MapFrom(src => (src.Telephone_Number == null || src.Telephone_Number == "undefined" ? string.Empty : src.Telephone_Number)))
                .ForMember(x => x.departmentId, opt => opt.MapFrom(src => src.PositionEntity.DepartmentEntity.Department_ID))
                .ForMember(x => x.departmentName, opt => opt.MapFrom(src => src.PositionEntity.DepartmentEntity.Department_Name))
                .ForMember(x => x.officeId, opt => opt.MapFrom(src => src.OfficeEntity.Office_ID))
                .ForMember(x => x.officeName, opt => opt.MapFrom(src => src.OfficeEntity.Office_Name))
                .ForMember(x => x.positionId, opt => opt.MapFrom(src => src.PositionEntity.Position_ID))
                .ForMember(x => x.positionName, opt => opt.MapFrom(src => src.PositionEntity.Position_Name))
                .ForMember(x => x.campusId, opt => opt.MapFrom(src => src.PositionEntity.DepartmentEntity.CampusEntity.Campus_ID))
                .ForMember(x => x.campusName, opt => opt.MapFrom(src => src.PositionEntity.DepartmentEntity.CampusEntity.Campus_Name))
                .ForMember(x => x.emergencyAddress, opt => opt.MapFrom(src => src.EmergencyContactEntity.Address))
                .ForMember(x => x.emergencyFullname, opt => opt.MapFrom(src => src.EmergencyContactEntity.Full_Name))
                .ForMember(x => x.emergencyRelationship, opt => opt.MapFrom(src => src.EmergencyContactEntity.Relationship))
                .ForMember(x => x.emergencyContact, opt => opt.MapFrom(src => src.EmergencyContactEntity.Contact_Number));

            CreateMap<personEntity, personOtherAccessTableVM>()
                .ForMember(x => x.status, opt => opt.MapFrom(src => src.IsActive == true ? "Active" : "Inactive"))
                .ForMember(x => x.personId, opt => opt.MapFrom(src => src.Person_ID))
                .ForMember(x => x.idNumber, opt => opt.MapFrom(src => src.ID_Number))
                .ForMember(x => x.firstName, opt => opt.MapFrom(src => src.First_Name))
                .ForMember(x => x.middleName, opt => opt.MapFrom(src => (src.Middle_Name == null || src.Middle_Name == "undefined" ? string.Empty : src.Middle_Name)))
                .ForMember(x => x.lastName, opt => opt.MapFrom(src => src.Last_Name))
                .ForMember(x => x.gender, opt => opt.MapFrom(src => (src.Gender.ToLower() == "m" ? "Male" : "Female")))
                .ForMember(x => x.birthdate, opt => opt.MapFrom(src => Convert.ToDateTime(src.Birthdate).ToString("yyyy-MM-dd")))
                .ForMember(x => x.address, opt => opt.MapFrom(src => src.Address))
                .ForMember(x => x.emailAddress, opt => opt.MapFrom(src => (src.Email_Address == null || src.Email_Address == "undefined" ? string.Empty : src.Email_Address)))
                .ForMember(x => x.contactNumber, opt => opt.MapFrom(src => src.Contact_Number))
                .ForMember(x => x.telephoneNumber, opt => opt.MapFrom(src => (src.Telephone_Number == null || src.Telephone_Number == "undefined" ? string.Empty : src.Telephone_Number)))
                .ForMember(x => x.departmentId, opt => opt.MapFrom(src => src.PositionEntity.DepartmentEntity.Department_ID))
                .ForMember(x => x.departmentName, opt => opt.MapFrom(src => src.PositionEntity.DepartmentEntity.Department_Name))
                .ForMember(x => x.officeId, opt => opt.MapFrom(src => src.OfficeEntity.Office_ID))
                .ForMember(x => x.officeName, opt => opt.MapFrom(src => src.OfficeEntity.Office_Name))
                .ForMember(x => x.positionId, opt => opt.MapFrom(src => src.PositionEntity.Position_ID))
                .ForMember(x => x.positionName, opt => opt.MapFrom(src => src.PositionEntity.Position_Name))
                .ForMember(x => x.campusId, opt => opt.MapFrom(src => src.PositionEntity.DepartmentEntity.CampusEntity.Campus_ID))
                .ForMember(x => x.campusName, opt => opt.MapFrom(src => src.PositionEntity.DepartmentEntity.CampusEntity.Campus_Name))
                .ForMember(x => x.emergencyAddress, opt => opt.MapFrom(src => src.EmergencyContactEntity.Address))
                .ForMember(x => x.emergencyFullname, opt => opt.MapFrom(src => src.EmergencyContactEntity.Full_Name))
                .ForMember(x => x.emergencyRelationship, opt => opt.MapFrom(src => src.EmergencyContactEntity.Relationship))
                .ForMember(x => x.emergencyContact, opt => opt.MapFrom(src => src.EmergencyContactEntity.Contact_Number));

            CreateMap<personOtherAccessVM, personEntity>()
                .ForMember(x => x.Person_ID, opt => opt.MapFrom(src => src.personId))
                .ForMember(x => x.ID_Number, opt => opt.MapFrom(src => src.idNumber))
                .ForMember(x => x.First_Name, opt => opt.MapFrom(src => src.firstName))
                .ForMember(x => x.Middle_Name, opt => opt.MapFrom(src => (src.middleName == null || src.middleName == "undefined" ? string.Empty : src.middleName)))
                .ForMember(x => x.Last_Name, opt => opt.MapFrom(src => src.lastName))
                .ForMember(x => x.Gender, opt => opt.MapFrom(src => (src.gender.ToLower() == "female" ? "F" : "M")))
                .ForMember(x => x.Birthdate, opt => opt.MapFrom(src => (src.birthdate == null ? DateTime.Now : Convert.ToDateTime(src.birthdate))))
                .ForMember(x => x.Address, opt => opt.MapFrom(src => src.address))
                .ForMember(x => x.Email_Address, opt => opt.MapFrom(src => (src.emailAddress == null || src.emailAddress == "undefined" ? string.Empty : src.emailAddress)))
                .ForMember(x => x.Contact_Number, opt => opt.MapFrom(src => (src.contactNumber == null ? string.Empty : src.contactNumber)))
                .ForMember(x => x.Telephone_Number, opt => opt.MapFrom(src => (src.telephoneNumber == null ? string.Empty : src.telephoneNumber)))
                .ForMember(x => x.Department_ID, opt => opt.MapFrom(src => src.departmentId))
                .ForMember(x => x.Office_ID, opt => opt.MapFrom(src => src.officeId))
                .ForMember(x => x.Position_ID, opt => opt.MapFrom(src => src.positionId))
                .ForMember(x => x.Campus_ID, opt => opt.MapFrom(src => src.campusId));

            #endregion

            #region CardMapping

            CreateMap<cardVM, cardDetailsEntity>()
                .ForMember(x => x.Cardholder_ID, opt => opt.MapFrom(src => src.cardId))
                .ForMember(x => x.Card_Serial, opt => opt.MapFrom(src => src.cardSerial))
                .ForMember(x => x.Card_Person_Type, opt => opt.MapFrom(src => src.cardPersonType))
                .ForMember(x => x.Expiry_Date, opt => opt.MapFrom(src => src.expiryDate))
                .ForMember(x => x.Remarks, opt => opt.MapFrom(src => src.remarks))
                .ForMember(x => x.Uid, opt => opt.MapFrom(src => src.uid))
                .ForMember(x => x.Person_ID, opt => opt.MapFrom(src => src.personId))
                .ForMember(x => x.Card_Is_Separated, opt => opt.MapFrom(src => src.isSeparated))
                .ForMember(x => x.Card_Separated_Date, opt => opt.MapFrom(src => src.separatedDate));

            CreateMap<cardDetailsEntity, cardVM>()
                 .ForMember(x => x.cardId, opt => opt.MapFrom(src => src.Cardholder_ID))
                 .ForMember(x => x.cardSerial, opt => opt.MapFrom(src => src.Card_Serial))
                 .ForMember(x => x.personId, opt => opt.MapFrom(src => src.Person_ID))
                 .ForMember(x => x.cardPersonType, opt => opt.MapFrom(src => src.Card_Person_Type))
                 .ForMember(x => x.issuedDate, opt => opt.MapFrom(src => src.Issued_Date))
                 .ForMember(x => x.expiryDate, opt => opt.MapFrom(src => src.Expiry_Date))
                 .ForMember(x => x.pan, opt => opt.MapFrom(src => src.PAN))
                 .ForMember(x => x.remarks, opt => opt.MapFrom(src => src.Remarks))
                 .ForMember(x => x.uid, opt => opt.MapFrom(src => src.Uid))
                 .ForMember(x => x.isActive, opt => opt.MapFrom(src => Convert.ToBoolean(src.IsActive)));

            //CreateMap<cardVM, tbl_card_details>()
            //    .ForMember(x => x.ID, opt => opt.MapFrom(src => src.Card_ID));

            #endregion

            #region TerminalWhitelist

            //int? isNull = null;
            //CreateMap<tbl_terminal_whitelist, terminalWhitelistVM>()
            //   .ForMember(x => x.Terminal_ID, opt => opt.MapFrom(src => src.Terminal_ID))
            //   .ForMember(x => x.Card_Details_ID, opt => opt.MapFrom(src => src.tbl_card_details.ID))
            //   .ForMember(x => x.ID_Number, opt => opt.MapFrom(src => src.tbl_card_details.tbl_person.Person_ID))
            //   .ForMember(x => x.Full_Name, opt => opt.MapFrom(src => src.tbl_card_details.tbl_person.First_Name + " " + src.tbl_card_details.tbl_person.Last_Name))
            //   .ForMember(x => x.Card_Serial, opt => opt.MapFrom(src => src.tbl_card_details.Card_Serial))
            //   .ForMember(x => x.Status, opt => opt.MapFrom(src => src.IsActive ?
            //        (src.tbl_card_details.tbl_data_sync.ID == isNull ? "Processing" : "Active") :
            //        (src.tbl_card_details.tbl_data_sync.ID == isNull ? "Processing" : "Inactive")));

            #endregion

            #region NotificationMapping

            CreateMap<notificationVM, notificationEntity>()
                .ForMember(x => x.Notification_ID, opt => opt.MapFrom(src => src.notificationId))
                .ForMember(x => x.Notification_Message, opt => opt.MapFrom(src => src.notificationMessage))
                .ForMember(x => x.Date_To_Display_From, opt => opt.MapFrom(src => Convert.ToDateTime(src.dateToDisplayFrom + " 00:00:00")))
                .ForMember(x => x.Date_To_Display_To, opt => opt.MapFrom(src => Convert.ToDateTime(src.dateToDisplayTo + " 23:59:59")))
                .ForMember(x => x.Person_ID, opt => opt.MapFrom(src => src.personId))
                .ForMember(x => x.Terminal_ID, opt => opt.MapFrom(src => src.terminalId))
                .ForMember(x => x.GUID, opt => opt.MapFrom(src => src.guid))
                .ForMember(x => x.Added_By, opt => opt.MapFrom(src => src.addedBy))
                .ForMember(x => x.Updated_By, opt => opt.MapFrom(src => src.updatedBy))
                .ForMember(x => x.IsActive, opt => opt.MapFrom(src => src.isActive))
                .ForMember(x => x.ToDisplay, opt => opt.MapFrom(src => src.toDisplay));

            CreateMap<notificationEntity, notificationVM>()
                .ForMember(x => x.notificationId, opt => opt.MapFrom(src => src.Notification_ID))
                .ForMember(x => x.notificationMessage, opt => opt.MapFrom(src => src.Notification_Message))
                .ForMember(x => x.dateToDisplayFrom, opt => opt.MapFrom(src => Convert.ToDateTime(src.Date_To_Display_From).ToString("yyyy-MM-dd")))
                .ForMember(x => x.dateToDisplayTo, opt => opt.MapFrom(src => Convert.ToDateTime(src.Date_To_Display_To).ToString("yyyy-MM-dd")))
                .ForMember(x => x.guid, opt => opt.MapFrom(src => src.GUID))
                .ForMember(x => x.personId, opt => opt.MapFrom(src => src.Person_ID))
                .ForMember(x => x.idNumber, opt => opt.MapFrom(src => src.PersonEntity.ID_Number))
                .ForMember(x => x.firstName, opt => opt.MapFrom(src => src.PersonEntity.First_Name))
                .ForMember(x => x.middleName, opt => opt.MapFrom(src => src.PersonEntity.Middle_Name))
                .ForMember(x => x.lastName, opt => opt.MapFrom(src => src.PersonEntity.Last_Name))
                .ForMember(x => x.terminalId, opt => opt.MapFrom(src => src.Terminal_ID))
                .ForMember(x => x.terminalName, opt => opt.MapFrom(src => Encryption.Decrypt(src.TerminalEntity.Terminal_Name)))
                .ForMember(x => x.areaId, opt => opt.MapFrom(src => src.TerminalEntity.Area_ID))
                .ForMember(x => x.areaName, opt => opt.MapFrom(src => src.TerminalEntity.AreaEntity.Area_Name))
                .ForMember(x => x.campusId, opt => opt.MapFrom(src => src.TerminalEntity.AreaEntity.Campus_ID))
                .ForMember(x => x.campusName, opt => opt.MapFrom(src => src.TerminalEntity.AreaEntity.CampusEntity.Campus_Name))
                .ForMember(x => x.addedBy, opt => opt.MapFrom(src => src.Added_By))
                .ForMember(x => x.updatedBy, opt => opt.MapFrom(src => src.Updated_By))
                .ForMember(x => x.isActive, opt => opt.MapFrom(src => src.IsActive))
                .ForMember(x => x.toDisplay, opt => opt.MapFrom(src => src.ToDisplay));

            CreateMap<personEntity, personNotificationVM>()
                .ForMember(x => x.personName, opt => opt.MapFrom(src => (src.ID_Number + " - " + src.Last_Name + ", " + src.First_Name)))
                .ForMember(x => x.personId, opt => opt.MapFrom(src => src.Person_ID));

            CreateMap<notificationPagedResult, notificationPagedResultVM>();

            #endregion

            #region Schedule Mapping

            CreateMap<scheduleVM, scheduleEntity>()
                .ForMember(x => x.Schedule_ID, opt => opt.MapFrom(src => src.scheduleId))
                .ForMember(x => x.Schedule_Name, opt => opt.MapFrom(src => src.scheduleName))
                .ForMember(x => x.Schedule_Status, opt => opt.MapFrom(src => src.scheduleStatus))
                .ForMember(x => x.Schedule_Days, opt => opt.MapFrom(src => src.scheduleDays))
                .ForMember(x => x.Schedule_Time_From, opt => opt.MapFrom(src => src.scheduleTimeFrom))
                .ForMember(x => x.Schedule_Time_To, opt => opt.MapFrom(src => src.scheduleTimeTo))
                .ForMember(x => x.Added_By, opt => opt.MapFrom(src => src.addedBy))
                .ForMember(x => x.Updated_By, opt => opt.MapFrom(src => src.updatedBy))
                .ForMember(x => x.IsActive, opt => opt.MapFrom(src => src.isActive))
                .ForMember(x => x.ToDisplay, opt => opt.MapFrom(src => src.toDisplay));

            CreateMap<scheduleEntity, scheduleVM>()
                .ForMember(x => x.scheduleId, opt => opt.MapFrom(src => src.Schedule_ID))
                .ForMember(x => x.scheduleName, opt => opt.MapFrom(src => src.Schedule_Name))
                .ForMember(x => x.scheduleStatus, opt => opt.MapFrom(src => src.Schedule_Status))
                .ForMember(x => x.scheduleDays, opt => opt.MapFrom(src => src.Schedule_Days.Remove(src.Schedule_Days.Length - 1)))
                .ForMember(x => x.scheduleTimeFrom, opt => opt.MapFrom(src => src.Schedule_Time_From))
                .ForMember(x => x.scheduleTimeTo, opt => opt.MapFrom(src => src.Schedule_Time_To))
                .ForMember(x => x.addedBy, opt => opt.MapFrom(src => src.Added_By))
                .ForMember(x => x.updatedBy, opt => opt.MapFrom(src => src.Updated_By))
                .ForMember(x => x.isActive, opt => opt.MapFrom(src => src.IsActive))
                .ForMember(x => x.toDisplay, opt => opt.MapFrom(src => src.ToDisplay));

            CreateMap<fetcherScheduleEntity, fetcherScheduleVM>()
                .ForMember(x => x.scheduleName, opt => opt.MapFrom(src => src.ScheduleEntity.Schedule_Name))
                .ForMember(x => x.scheduleDays, opt => opt.MapFrom(src => src.ScheduleEntity.Schedule_Days))
                .ForMember(x => x.scheduleTimeFrom, opt => opt.MapFrom(src => src.ScheduleEntity.Schedule_Time_From))
                .ForMember(x => x.scheduleTimeTo, opt => opt.MapFrom(src => src.ScheduleEntity.Schedule_Time_To))
                .ForMember(x => x.scheduleId, opt => opt.MapFrom(src => src.Schedule_ID))
                .ForMember(x => x.fetcherId, opt => opt.MapFrom(src => src.Fetcher_ID))
                .ForMember(x => x.fetcherSchedId, opt => opt.MapFrom(src => src.Fetcher_Sched_ID));

            CreateMap<fetcherScheduleVM, fetcherScheduleEntity> ()
                .ForMember(x => x.Schedule_ID, opt => opt.MapFrom(src => src.scheduleId))
                .ForMember(x => x.Fetcher_ID, opt => opt.MapFrom(src => src.fetcherId))
                .ForMember(x => x.Fetcher_Sched_ID, opt => opt.MapFrom(src => src.fetcherSchedId));
            
            CreateMap<personEntity, fetcherScheduleDetailsEntity>()
                .ForMember(x => x.Person_ID, opt => opt.MapFrom(src => src.Person_ID));

            CreateMap<fetcherGroupEntity, fetcherScheduleDetailsEntity>()
                .ForMember(x => x.Fetcher_Group_ID, opt => opt.MapFrom(src => src.Fetcher_Group_ID));

            CreateMap<fetcherScheduleDetailsEntity, fetcherScheduleDetailsVM>()
                .ForMember(x => x.scheduleName, opt => opt.MapFrom(src => src.FetcherScheduleEntity.ScheduleEntity.Schedule_Name))
                .ForMember(x => x.scheduleDays, opt => opt.MapFrom(src => src.FetcherScheduleEntity.ScheduleEntity.Schedule_Days))
                .ForMember(x => x.scheduleTimeFrom, opt => opt.MapFrom(src => src.FetcherScheduleEntity.ScheduleEntity.Schedule_Time_From))
                .ForMember(x => x.scheduleTimeTo, opt => opt.MapFrom(src => src.FetcherScheduleEntity.ScheduleEntity.Schedule_Time_To))
                .ForMember(x => x.studentName, opt => opt.MapFrom(src => (src.PersonEntity.ID_Number + " - " + src.PersonEntity.Last_Name + ", " + src.PersonEntity.First_Name)))
                .ForMember(x => x.groupName, opt => opt.MapFrom(src => src.FetcherGroupEntity.Group_Name))
                .ForMember(x => x.personId, opt => opt.MapFrom(src => src.Person_ID))
                .ForMember(x => x.fetcherSchedId, opt => opt.MapFrom(src => src.Fetcher_Sched_ID))
                .ForMember(x => x.fetcherGroupId, opt => opt.MapFrom(src => src.Fetcher_Group_ID))
                .ForMember(x => x.fetcherSchedDtlId, opt => opt.MapFrom(src => src.Fetcher_Sched_Dtl_ID));

            CreateMap<fetcherScheduleDetailsVM, fetcherScheduleDetailsEntity>()
                .ForMember(x => x.Person_ID, opt => opt.MapFrom(src => src.personId))
                .ForMember(x => x.Fetcher_Group_ID, opt => opt.MapFrom(src => src.fetcherGroupId))
                .ForMember(x => x.Fetcher_Sched_ID, opt => opt.MapFrom(src => src.fetcherSchedId))
                .ForMember(x => x.Fetcher_Sched_Dtl_ID, opt => opt.MapFrom(src => src.fetcherSchedDtlId));

            CreateMap<schedulePagedResult, schedulePagedResultVM>();


            #endregion

            #region Fetcher Group Mapping

            CreateMap<fetcherGroupVM, fetcherGroupEntity>()
                .ForMember(x => x.Fetcher_Group_ID, opt => opt.MapFrom(src => src.fetcherGroupId))
                .ForMember(x => x.Group_Name, opt => opt.MapFrom(src => src.groupName))
                .ForMember(x => x.Campus_ID, opt => opt.MapFrom(src => src.campusId))
                .ForMember(x => x.Added_By, opt => opt.MapFrom(src => src.addedBy))
                .ForMember(x => x.Updated_By, opt => opt.MapFrom(src => src.updatedBy))
                .ForMember(x => x.IsActive, opt => opt.MapFrom(src => src.isActive))
                .ForMember(x => x.ToDisplay, opt => opt.MapFrom(src => src.toDisplay));

            CreateMap<fetcherGroupEntity, fetcherGroupVM>()
                .ForMember(x => x.fetcherGroupId, opt => opt.MapFrom(src => src.Fetcher_Group_ID))
                .ForMember(x => x.groupName, opt => opt.MapFrom(src => src.Group_Name))
                .ForMember(x => x.campusId, opt => opt.MapFrom(src => src.Campus_ID))
                .ForMember(x => x.campusName, opt => opt.MapFrom(src => src.CampusEntity.Campus_Name))
                .ForMember(x => x.addedBy, opt => opt.MapFrom(src => src.Added_By))
                .ForMember(x => x.updatedBy, opt => opt.MapFrom(src => src.Updated_By))
                .ForMember(x => x.isActive, opt => opt.MapFrom(src => src.IsActive))
                .ForMember(x => x.toDisplay, opt => opt.MapFrom(src => src.ToDisplay));

            CreateMap<fetcherGroupPagedResult, fetcherGroupPagedResultVM>();

            #endregion

            #region Fetcher Group Details Mapping

            CreateMap<fetcherGroupDetailsVM, fetcherGroupDetailsEntity>()
                .ForMember(x => x.Group_Detail_ID, opt => opt.MapFrom(src => src.groupDetailId))
                .ForMember(x => x.Fetcher_Group_ID, opt => opt.MapFrom(src => src.fetcherGroupID))
                .ForMember(x => x.Person_ID, opt => opt.MapFrom(src => src.personId))
                .ForMember(x => x.Added_By, opt => opt.MapFrom(src => src.addedBy))
                .ForMember(x => x.Updated_By, opt => opt.MapFrom(src => src.updatedBy))
                .ForMember(x => x.IsActive, opt => opt.MapFrom(src => src.isActive))
                .ForMember(x => x.ToDisplay, opt => opt.MapFrom(src => src.toDisplay));

            CreateMap<fetcherGroupDetailsEntity, fetcherGroupDetailsVM>()
                .ForMember(x => x.groupDetailId, opt => opt.MapFrom(src => src.Group_Detail_ID))
                .ForMember(x => x.fetcherGroupID, opt => opt.MapFrom(src => src.Fetcher_Group_ID))
                .ForMember(x => x.groupName, opt => opt.MapFrom(src => src.FetcherGroupEntity.Group_Name))
                .ForMember(x => x.personId, opt => opt.MapFrom(src => src.Person_ID))
                .ForMember(x => x.idNumber, opt => opt.MapFrom(src => src.PersonEntity.ID_Number))
                .ForMember(x => x.firstName, opt => opt.MapFrom(src => src.PersonEntity.First_Name))
                .ForMember(x => x.lastName, opt => opt.MapFrom(src => src.PersonEntity.Last_Name))
                .ForMember(x => x.addedBy, opt => opt.MapFrom(src => src.Added_By))
                .ForMember(x => x.updatedBy, opt => opt.MapFrom(src => src.Updated_By))
                .ForMember(x => x.isActive, opt => opt.MapFrom(src => src.IsActive))
                .ForMember(x => x.toDisplay, opt => opt.MapFrom(src => src.ToDisplay));

            CreateMap<personEntity, fetcherStudentsVM>()
                .ForMember(x => x.personId, opt => opt.MapFrom(src => src.Person_ID))
                .ForMember(x => x.fetcherStudents, opt => opt.MapFrom(src => (src.ID_Number + " - " + src.Last_Name + ", " + src.First_Name)));

            CreateMap<fetcherGroupDetailsPagedResult, fetcherGroupDetailsPagedResultVM>();

            #endregion
        }
    }
}

namespace ApplicationWebPointeuse.Common
{
    public static class Constants
    {
        // GLOBAL TIERCES APPS CONSTANTS

        // SERVER
        public const string LOCALHOST_URL = "https://localhost:";

        // API
        public const string API_PORT = "7026";
        public const string API_URI = "/api";
        public const string API_GLOBAL_URI = LOCALHOST_URL + API_PORT + API_URI;

        // GLOBAL API METHODS CONSTANTS

        // ArrivalDateTime
        public const string ARRIVAL_DATE_TIME_MODEL = "/ArrivalDateTime";
        // GET
        public const string GET_LIST_ADT = "/getArrivalDateTimeList";
        public const string GET_ALL_LIST_ADT = "/getAllArrivalDateTimeList";
        public const string GET_ID_ADT = "/getArrivalDateTime/";
        public const string GET_ALL_ID_ADT = "/getAllArrivalDateTime/";
        // POST
        public const string POST_ADT = "/addArrivalDateTime";
        // PUT
        public const string PUT_ADT = "/updateArrivalDateTime";
        // DELETE
        public const string DELETE_ADT = "/deleteArrivalDateTime";
        public const string SOFT_DELETE_ADT = "/softDeleteArrivalDateTime";

        // Cycles
        public const string CYCLES_MODEL = "/Cycles";
        // GET
        public const string GET_LIST_CYCLES = "/getCyclesList";
        public const string GET_ALL_LIST_CYCLES = "/getAllCyclesList";
        public const string GET_ID_CYCLE = "/getCycle/";
        public const string GET_ALL_ID_CYCLE = "/getAllCycle/";
        // POST
        public const string POST_CYCLE = "/addCycle";
        // PUT
        public const string PUT_CYCLE = "/updateCycle";
        // DELETE
        public const string DELETE_CYCLE = "/deleteCycle";
        public const string SOFT_DELETE_CYCLE = "/softDeleteCycle";

        // Periods
        public const string PERIODS_MODEL = "/Periods";
        // GET
        public const string GET_LIST_PERIODS = "/getPeriodsList";
        public const string GET_ALL_LIST_PERIODS = "/getAllPeriodsList";
        public const string GET_ID_PERIOD = "/getPeriod/";
        public const string GET_ALL_ID_PERIOD = "/getAllPeriod/";
        // POST
        public const string POST_PERIOD = "/addPeriod";
        // PUT
        public const string PUT_PERIOD = "/updatePeriod";
        // DELETE
        public const string DELETE_PERIOD = "/deletePeriod";
        public const string SOFT_DELETE_PERIOD = "/softDeletePeriod";

        // Schoolclasses
        public const string SCHOOLCLASSES_MODEL = "/Schoolclasses";
        // GET
        public const string GET_LIST_SCHOOLCLASSES = "/getSchoolclassesList";
        public const string GET_ALL_LIST_SCHOOLCLASSES = "/getAllSchoolclassesList";
        public const string GET_ID_SCHOOLCLASS = "/getSchoolclass/";
        public const string GET_ALL_ID_SCHOOLCLASS = "/getAllSchoolclass/";

        public const string GET_SECTION_SCHOOLCLASSES = "/Section/";
        public const string GET_CYCLE_SCHOOLCLASSES = "/Cycle/";
        public const string GET_SUBSECTION_SCHOOLCLASSES = "/Subsection/";
        // POST
        public const string POST_SCHOOLCLASS = "/addSchoolclass";
        // PUT
        public const string PUT_SCHOOLCLASS = "/updateSchoolclass";
        // DELETE
        public const string DELETE_SCHOOLCLASS = "/deleteSchoolclass";
        public const string SOFT_DELETE_SCHOOLCLASS = "/softDeleteSchoolclass";

        // Sections
        public const string SECTIONS_MODEL = "/Sections";
        // GET
        public const string GET_LIST_SECTIONS = "/getSectionsList";
        public const string GET_ALL_LIST_SECTIONS = "/getAllSectionsList";
        public const string GET_ID_SECTION = "/getSection/";
        public const string GET_ALL_ID_SECTION = "/getAllSection/";
        // POST
        public const string POST_SECTION = "/addSection";
        // PUT
        public const string PUT_SECTION = "/updateSection";
        // DELETE
        public const string DELETE_SECTION = "/deleteSection";
        public const string SOFT_DELETE_SECTION = "/softDeleteSection";

        // Students
        public const string STUDENTS_MODEL = "/Students";
        // GET
        public const string GET_LIST_STUDENTS = "/getStudentsList";
        public const string GET_ALL_LIST_STUDENTS = "/getAllStudentsList";
        public const string GET_ID_STUDENT = "/getStudent/";
        public const string GET_STUDENTS = "/getStudents";
        public const string GET_ALL_ID_STUDENT = "/getAllStudent/";
        // POST
        public const string POST_STUDENT = "/addStudent";
        // PUT
        public const string PUT_STUDENT = "/updateStudent";
        // DELETE
        public const string DELETE_STUDENT = "/deleteStudent";
        public const string SOFT_DELETE_STUDENT = "/softDeleteStudent";

        // StudentsDevice
        public const string STUDENTS_DEVICE_MODEL = "/StudentsDevice";
        // GET
        public const string GET_LIST_STUDENTSDEVICE = "/getStudentsDeviceList";
        public const string GET_ALL_LIST_STUDENTSDEVICE = "/getAllStudentsDeviceList";
        public const string GET_ID_STUDENTDEVICE = "/getStudentDevice/";
        public const string GET_ALL_ID_STUDENTDEVICE = "/getAllStudentDevice/";
        // POST
        public const string POST_STUDENTDEVICE = "/addStudentDevice";
        // PUT
        public const string PUT_STUDENTDEVICE = "/updateStudentDevice";
        // DELETE
        public const string DELETE_STUDENTDEVICE = "/deleteStudentDevice";
        public const string SOFT_DELETE_STUDENTDEVICE = "/softDeleteStudentDevice";

        // Subsections
        public const string SUBSECTIONS_MODEL = "/Subsections";
        // GET
        public const string GET_LIST_SUBSECTIONS = "/getSubsectionsList";
        public const string GET_ALL_LIST_SUBSECTIONS = "/getAllSubsectionsList";
        public const string GET_ID_SUBSECTION = "/getSubsection/";
        public const string GET_ALL_ID_SUBSECTION = "/getAllSubsection/";
        // POST
        public const string POST_SUBSECTION = "/addSubsection";
        // PUT
        public const string PUT_SUBSECTION = "/updateSubsection";
        // DELETE
        public const string DELETE_SUBSECTION = "/deleteSubsection";
        public const string SOFT_DELETE_SUBSECTION = "/softDeleteSubsection";
    }
}

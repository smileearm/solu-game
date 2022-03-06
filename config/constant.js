const Response = {
    test: { 
      a: 10,
    },
    Success: {
      Status: {
        code: 0,
        message: 'สำเร็จ'
      }
    },
    UsernameDuplicate: {
        Status: {
            code: 1,
            message: 'มีผู้ใช้ชื่อนี้แล้ว'
          }
    },
    Roomstart: {
        Status: {
            code: 2,
            message: 'ห้องได้ดำเนินการเล่นแล้ว กรุณารอเกมส์ใหม่'
          }
    },
    InvalidPassword: {
        Status: {
            code: 3,
            message: 'กรอกรหัสพาสเวิร์ดผิด กรุณากรอกใหม่'
          }
    },
    NoRoom: {
        Status: {
            code: 4,
            message: 'ยังไม่มีห้อง'
          }
    },
    PlayerLower: {
        Status: {
            code: 5,
            message: 'ผู้เล่นน้อยกว่าขั้นต่ำของห้อง'
          }
    },
    PlayerMaximum: {
        Status: {
            code: 6,
            message: 'ผู้เล่นห้องเต็ม'
          }
    },
    CreatorOut: {
        Status: {
            code: 7,
            message: 'คนสร้างออกจากห้อง'
          }
    },
    
   

}
exports.Response = Response

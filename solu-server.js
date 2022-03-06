
const express = require('express')
var app = require('express')()
var http = require('http').Server(app)
var io = require('socket.io')(http)
var port = process.env.PORT || 3000
const Constants = require('./config/constant')
var fs = require('fs')
// var datajson = fs.readFileSync('question.json');
var datajson = fs.readFileSync('test.json');

var question = JSON.parse(datajson);
// var compression = require('compression');

// app.use(compression());
// app.use(express.static(__dirname + '/public/'))
let rooms = {}



let choice = []
let timer = { countdown: {} }
let problems = [0, 1, 2]
let usernamePlayers = []
let countdown = 9
let socketId = []
let player = {}
let name
let timeSkip = 0
let playerWaitRoom = []

io.on('connection', socket => {
  console.log('dasklfjladsjflk')
  console.log(socket.id + ' Connected')

  // SCENE 1 (START SCENE)

  socket.on('checkplayername', (data, callback) => {
    console.log(socket)
    console.log('--------------------checkUsername--------------------')
    console.log('DATA: ', data, 'SOCKET ID:',  socket.id)
    let checkNameCreator, isCheckNamePlayer = false
    if (Object.keys(rooms).length) {
      for (let i = 0; i < Object.keys(rooms).length; i++) {
        checkNameCreator = rooms[Object.keys(rooms)[i]].creator.name
        rooms[Object.keys(rooms)[i]].players.filter(v => {
          if (v.name === data.username) {
            checkUsername = true
          }
        })
        if (checkNameCreator === data.username || isCheckNamePlayer) {
          callback({ status: Constants.Response.UsernameDuplicate.Status })
        } else {
          if (playerWaitRoom.indexOf(data.username) === -1) {
            playerWaitRoom.push(data.username)
            callback({ status: Constants.Response.Success.Status, data: { username: data.username } })
          } else {
            callback({ status: Constants.Response.UsernameDuplicate.Status })
          }
        }
      }
    } else {
      if (playerWaitRoom.indexOf(data.username) === -1) {
        playerWaitRoom = [...playerWaitRoom, data.username]
        console.log({ status: Constants.Response.Success.Status, data: { username: data.username } })
        callback({ status: Constants.Response.Success.Status, data: { username: data.username } })
        // callback({ status: Constants.Response.Success.Status, username: data.username })
      } else {
        callback({ status: Constants.Response.UsernameDuplicate.Status })
      }
    }
  })

  // SCENE 2 (LOBBY SCENE)
  
  socket.on('createroom', (data, callback) => {
    console.log(socket)
    console.log('--------------------createroom--------------------')
    console.log('DATA: ', data, 'SOCKET ID:',  socket.id)
    const numberRoom = 'p' + data.randomPassword
    if (!rooms[numberRoom]) {
      RandomChoiceQuestion()
      rooms[numberRoom] = {
        creator: {},
        players: [],
        password: data.randomPassword,
        maxPlayers: 9,
        minPlayers: 1,
        numberPlayer: 0,
        timeInRoom: 0,
        isActive: false,
        questionInRoom: choice,
        numberQuestion: 0,
        skipGame: false
      }
    }
    console.log('playerWaitRoom ', playerWaitRoom)
    playerWaitRoom = playerWaitRoom.filter(v => data.playerName.indexOf(v) === -1)
    console.log('playerWaitRoom ', playerWaitRoom)
    rooms[numberRoom].creator = {
      id: socket.id,
      name: data.playerName,
      status: data.status,
    }
    callback({ status: Constants.Response.Success.Status, data: rooms[numberRoom].creator })
  })

  socket.on('joinroom', (data, callback) => {
    console.log('--------------------joinroom--------------------')
    console.log('DATA: ', data, 'SOCKET ID:',  socket.id)
    const numberRoom = 'p' + data.randomPassword
    rooms[numberRoom].players[socket.id] = {
      id: socket.id,
      name: data.playerName,
      status: 'player',
      score: 0,
      timeAnswer: 0,
      type: 'in'
    }
    rooms[numberRoom].numberPlayer = Object.keys(rooms[numberRoom].players).length
    let players = []
    for (const key in rooms[numberRoom].players) {
      players = [...players, rooms[numberRoom].players[key]]
    }
    rooms[numberRoom].players = players
    const response = {
      name: data.playerName,
      numberPlayer: rooms[numberRoom].numberPlayer,
      id: socket.id,
      type: 'in'
    }
    socket.join(numberRoom)
    io.to(numberRoom).emit('playerupdate', { status: Constants.Response.Success.Status, data: response })
    io.to(rooms[numberRoom].creator.id).emit('playerupdate', { status: Constants.Response.Success.Status, data: response })
    // callback({ status: Constants.Response.Success.Status, data: rooms[numberRoom] })
  })



  //Start
  socket.on('checkUsername', (data, callback) => {
    console.log('--------------------checkUsername--------------------')
    console.log('DATA: ', data, 'SOCKET ID:',  socket.id)
    let checkNameCreator, checkUsername = false
    if (Object.keys(rooms).length) {
      for (let i = 0; i < Object.keys(rooms).length; i++) {
        checkNameCreator = rooms[Object.keys(rooms)[i]].creator.name
        rooms[Object.keys(rooms)[i]].players.filter(v => {
          if (v.name === data.username) {
            checkUsername = true
          }
        })
        if (checkNameCreator === data.username || checkUsername) {
          callback({ status: Constants.Response.UsernameDuplicate.Status })
        } else {
          if (playerWaitRoom.indexOf(data.username) === -1) {
            playerWaitRoom.push(data.username)
            callback({ status: Constants.Response.Success.Status, username: data.username })
          } else {
            callback({ status: Constants.Response.UsernameDuplicate.Status })
          }
        }
      }
    } else {
      if (playerWaitRoom.indexOf(data.username) === -1) {
        playerWaitRoom = [...playerWaitRoom, data.username]
        callback({ status: Constants.Response.Success.Status, username: data.username })
      } else {
        callback({ status: Constants.Response.UsernameDuplicate.Status })
      }
    }
  })
  
  // Login 
  socket.on('checkpassword', (data, callback) => {
    console.log('--------------------checkpassword--------------------')
    console.log('DATA: ', data, 'SOCKET ID:',  socket.id)
    let numberRoom = 'p' + data.password
    let isCheckRoom = false
    for (let key in rooms) {
      if (key === numberRoom) {
        isCheckRoom = true
      }
    }
    if (isCheckRoom && !rooms[numberRoom].isActive && rooms[numberRoom].maxPlayers > Object.keys(rooms[numberRoom].players).length) {
      playerWaitRoom = playerWaitRoom.filter(v => data.name.indexOf(v) === -1)
      callback({ status: Constants.Response.Success.Status })
    } else if (isCheckRoom && !rooms[numberRoom].isActive && rooms[numberRoom].maxPlayers === Object.keys(rooms[numberRoom].players).length) {
      callback({ status: Constants.Response.PlayerMaximum.Status })
    } else if (isCheckRoom && rooms[numberRoom].isActive) {
      callback({ status: Constants.Response.Roomstart.Status })
    } else {
      callback({ status: Constants.Response.InvalidPassword.Status })
    }
  })
  
  //Lobby
  // socket.on('createroom', (data, callback) => {
  //   console.log('--------------------createroom--------------------')
  //   console.log('DATA: ', data, 'SOCKET ID:',  socket.id)
  //   let numberRoom = 'p' + data.randomPassword
  //   if (!rooms[numberRoom]) {
  //     RandomChoiceQuestion()
  //     rooms[numberRoom] = {
  //       creator: {},
  //       players: [],
  //       password: data.randomPassword,
  //       maxPlayers: 9,
  //       minPlayers: 1,
  //       numberPlayer: 0,
  //       timeInRoom: 0,
  //       isActive: false,
  //       questionInRoom: choice,
  //       numberQuestion: 0,
  //       skipGame: false
  //     }
  //   }
  //   playerWaitRoom = playerWaitRoom.filter(v => data.playerName.indexOf(v) === -1)
  //   rooms[numberRoom].creator = {
  //     id: socket.id,
  //     name: data.playerName,
  //     status: data.status,
  //   }
  //   callback({ status: Constants.Response.Success.Status, data: rooms[numberRoom].creator })
  // })
  
  // socket.on('joinroom', (data, callback) => {
  //   console.log('--------------------joinroom--------------------')
  //   console.log('DATA: ', data, 'SOCKET ID:',  socket.id)
  //   let numberRoom = 'p' + data.randomPassword
  //   rooms[numberRoom].players[socket.id] = {
  //     id: socket.id,
  //     name: data.playerName,
  //     status: 'player',
  //     score: 0,
  //     timeAnswer: 0,
  //     type: 'in'
  //   }
  //   rooms[numberRoom].numberPlayer = Object.keys(rooms[numberRoom].players).length
  //   let players = []
  //   for (const key in rooms[numberRoom].players) {
  //     players = [...players, rooms[numberRoom].players[key]]
  //   }
  //   rooms[numberRoom].players = players
  //   let response = {
  //     name: data.playerName,
  //     numberPlayer: rooms[numberRoom].numberPlayer,
  //     id: socket.id,
  //     type: 'in'
  //   }
  //   console.log(JSON.stringify(rooms))
  //   io.to(numberRoom).emit('playerupdate', { status: Constants.Response.Success.Status, data: response })
  //   socket.join(numberRoom)
  //   io.to(rooms[numberRoom].creator.id).emit('playerupdate', { status: Constants.Response.Success.Status, data: response })
  //   callback({ status: Constants.Response.Success.Status, data: rooms[numberRoom] })
  // })
  
  socket.on('joinSocket', (data) => {
    console.log('=====> joinSocket')
    let numberRoom = 'p' + data
    socket.join(numberRoom)
    // socket.join(numberRoom, (data) => { console.log('join', data)
  })

  socket.on('testSortScore', (data, callback) => {
    console.log('---testSortScore---')
    console.log('data ', data)
    let numberRoom = 'p' + data
    // rooms[numberRoom].numberQuestion++
    rooms[numberRoom].skipGame = false
    // socket.join(numberRoom)
    const answerQuestionFirst = rooms[numberRoom].players.sort((a, b) => {
      if (a.score === b.score) {
        return b.time - a.time
      } else {
        return b.score - a.score
      }
    });
    io.to(numberRoom).emit('checkSort', { status: Constants.Response.Success.Status, data: answerQuestionFirst })
  })
  
  socket.on('startgame', (data, callback) => {
    console.log('--------------------startgame--------------------')
    console.log('DATA: ', data, 'SOCKET ID:',  socket.id)
    console.log('rooms =======> ', rooms)
    let numberRoom = 'p' + data
    if (Object.keys(rooms[numberRoom].players).length >= rooms[numberRoom].minPlayers) {
      rooms[numberRoom].isActive = true
      io.to(numberRoom).emit('changestartscene', '{}')
      io.to(rooms[numberRoom].creator.id).emit('changestartscene', '{}')
      callback({ status: Constants.Response.Success.Status})
    } else {
      callback({ status: Constants.Response.PlayerLower.Status})
        console.log('PLAYER LOWER IN ROOM')
    }
  })
    
    //Playing
    // socket.on('gamecontroller', (data, callback) => {
    socket.on('gamecontroller', data => {
      console.log('--------------------gamecontroller--------------------')
      console.log('DATA: ', data, 'SOCKET ID:',  socket.id)
      let numberRoom = 'p' + data
      socket.join(numberRoom)
      countdown = 9
      rooms[numberRoom].timeInRoom = 9
  
      if (!rooms[numberRoom].skipGame) {
        timer.countdown[numberRoom] = setInterval(() => {
          // timeSkip = rooms[numberRoom].timeInRoom--
          io.to(numberRoom).emit('countdownTime', { status: Constants.Response.Success.Status, data: { countdown: rooms[numberRoom].timeInRoom-- } })
          if (rooms[numberRoom].timeInRoom < 0) {
            clearInterval(timer.countdown[numberRoom])
            delete (timer.countdown[rooms[numberRoom]])
          }
        }, 1000)
      }
      // callback({ status: Constants.Response.Success.Status, data: {countdown: rooms[numberRoom].timeInRoom }})
    })
  
    socket.on('skipGame', data => {
      console.log('--------------------skipGame--------------------')
      console.log('DATA: ', data, 'SOCKET ID:',  socket.id)
      let numberRoom = 'p' + data
      rooms[numberRoom].timeInRoom = 0
      rooms[numberRoom].skipGame = true
      io.to(numberRoom).emit('skipallplayer', {})
    })
  
    socket.on('question', (data, callback) => {
      let numberRoom = 'p' + data
      let count = rooms[numberRoom].numberQuestion
      let response = {
        questionList: question.questionList[rooms[numberRoom].questionInRoom[count]],
        number: problems[count],
        numberProblems: choice[count],
        numberQuestion: count
      }
      callback({ status: Constants.Response.Success.Status, data: response })
    })
  
    socket.on('cancelgame', (data, callback) => {
      console.log('--------------------cancelgame--------------------')
      console.log('DATA: ', data, 'SOCKET ID:',  socket.id)
      let numberRoom = 'p' + data.password
      if (data.status === 'banker') {
        io.to(numberRoom).emit('changecancelscene', '{}')
        io.to(rooms[numberRoom].creator.id).emit('changecancelscene', '{}')
      } else {
        callback('Success')
      }
    })
  
    socket.on('playerout', (data, callback) => {
      console.log('--------------------playerout--------------------')
      console.log('DATA: ', data, 'SOCKET ID:',  socket.id)
      let numberRoom = 'p' + data.numberRoom
      if (data.status === 'player') {
        rooms[numberRoom].players.filter(x => {
          if (x.id.toString() === data.id.toString()) {
            x.type = 'out'
          }
        })
        rooms[numberRoom].numberPlayer--
      } else {
        if (rooms[numberRoom].creator.id.toString() === data.id.toString()) {
          io.to(numberRoom).emit('bankerOut', { status: Constants.Response.CreatorOut.Status })
        }
      }
      if (data.check === 'cancelgame') {
        callback(Constants.Response.Success.Status)
      }
    })
    socket.on('sortscore', (data, callback) => {
      console.log('--------------------sortscore--------------------')
      console.log('DATA: ', data, 'SOCKET ID:',  socket.id)
      let numberRoom = 'p' + data
      // rooms[numberRoom].numberQuestion++
      rooms[numberRoom].skipGame = false
      socket.join(numberRoom)

      const answerQuestionFirst = rooms[numberRoom].players.sort((a, b) => {
        if (a.score === b.score) {
          return b.time - a.time
        } else {
          return b.score - a.score
        }
      });
      callback({ status: Constants.Response.Success.Status, data: answerQuestionFirst })
    })
  
    socket.on('checkanswer', (data, callback) => {
      console.log('--------------------checkanswer--------------------')
      console.log('DATA: ', data, 'SOCKET ID:',  socket.id)
      let numberRoom = 'p' + data.numberRoom
      socket.join(numberRoom)
      for (let i = 0; i < rooms[numberRoom].players.length; i++) {
        if (rooms[numberRoom].players[i].name === data.name) {
          rooms[numberRoom].players[i].timeAnswer = rooms[numberRoom].timeInRoom
          if (question.questionList[choice[rooms[numberRoom].numberQuestion]].answer === data.id) rooms[numberRoom].players[i].score += 10 * rooms[numberRoom].timeInRoom
        }
      }
      io.to(numberRoom).emit('playerAnswer', {})
      callback({ status: Constants.Response.Roomstart.Status, data: rooms[numberRoom] })
    })
  
    socket.on('playerInRoom', (data, callback) => {
      console.log('--------------------playerInRoom--------------------')
      console.log('DATA: ', data, 'SOCKET ID:',  socket.id)
      let numberRoom = 'p' + data.numberRoom
      socket.join(numberRoom)
      if (data.status === 'banker') {
        rooms[numberRoom].players.filter((v, i) => {
          if (v.type === 'out') {
            delete rooms[numberRoom].players[i]
          }
        })
        let players = rooms[numberRoom].players.filter(el => {
          return Object.keys(el).length > 0;
        });
        rooms[numberRoom].players = players
      }
      rooms[numberRoom].players.filter(v => v.score = 0)
      rooms[numberRoom].isActive = false

      // if(data.status === 'banker') {
      //     // test = rooms[numberRoom].creator.id ===  data.id
      //     // console.log('ttt', rooms[numberRoom].creator)
      // } else {
      //     // console.log('--->', rooms[numberRoom].players[data.id])
      //     test = rooms[numberRoom].players.filter(v =>  v.id.toString() === data.id.toString())[0]
      //     console.log('test ', test)
      //     // console.log('test', test)
      //     // console.log('test', test.name)
  
      //     response = {
      //         name: test.name,
      //         numberPlayer: 222,
      //         id: test.id
      //     }
      //     io.to(numberRoom).emit('playerupdate', { status: Constants.Response.Success.Status, data: response })
  
      // }
      // io.to(numberRoom).emit('playerupdate', { status: Constants.Response.Success.Status, data: rooms[numberRoom].players})
      // io.to(rooms[numberRoom].creator.id).emit('playerupdate', { status: Constants.Response.Roomstart.Status, data: rooms[numberRoom].players })
      // rooms[numberRoom].isActive = false
      callback({ status: Constants.Response.Success.Status, data: rooms[numberRoom] })
    })
  
    socket.on('boardcasttoscence', (data, callback) => {
      console.log('--------------------boardcasttoscence--------------------')
      console.log('DATA: ', data, 'SOCKET ID:',  socket.id)
      let numberRoom = 'p' + data
      rooms[numberRoom].numberQuestion++
      io.to(numberRoom).emit('changestartscene', '{}')
    })
    socket.on('a123', data => {
      console.log('--------------------a123--------------------')
      console.log('DATA: ', data, 'SOCKET ID:',  socket.id)
      let numberRoom = 'p' + data
      io.to(numberRoom).emit('changestartscene', '{}')
    })

    // socket.on('exitroom', data => {
    //     let numberRoom = 'p' + data.numberRoom
    //     let userId = data.id
    //     // console.log('data', data)
    //     if(data.status === 'banker') {
    //       console.log('room', rooms[numberRoom])
  
    //       delete rooms[numberRoom] 
    //         console.log('room', rooms)
    //     } else {
    //         rooms[numberRoom].players.filter(v => {
    //             if (v.id.toString() === userId.toString()) {
    //                 delete v
    //             }
    //         })
    //         let players = rooms[numberRoom].players.filter(el => {
    //             return Object.keys(el).length > 0;
    //         });
    //         rooms[numberRoom].players = players
    //     }   
    // })
  
    socket.on('reScore', (data, callback) => {
      console.log('--------------------reScore--------------------')
      console.log('DATA: ', data, 'SOCKET ID:',  socket.id)
      console.log('rooms =======> ', rooms)
      
      socketId.push(socket.id)
      let numberRoom = 'p' + data
      for (let i = 0; i < rooms[numberRoom].players.length; i++) {
        rooms[numberRoom].players[i].score = 0
      }
      io.to(numberRoom).emit('changelobbyscene', '{}')
      rooms[numberRoom].isActive = false
      rooms[numberRoom].players.filter(v => {
        if (v.name.toString() === 'aaaa') {
          delete v.name
        }
      })
      let players = rooms[numberRoom].players.filter(el => {
        return Object.keys(el).length > 0;
      });
      rooms[numberRoom].players = players
    })
    socket.on('sockerUser', (data, callback) => {
      console.log('--------------------sockerUser--------------------')
      console.log('DATA: ', data, 'SOCKET ID:',  socket.id)
      socket.user = socket.id
    })
    socket.on('soketJoinroom', data => {
      let numberRoom = 'p' + data
      socket.join(numberRoom)
    })
    // socket.on('cancalgame', (data, callback) => {
    //   let numberRoom = 'p' + data.numberRoom
    //   io.to(numberRoom).emit('leaveRoom', {})
    //   socket.leave(numberRoom)
    // })

    socket.on('exitroom', (data, callback) => {
      console.log('--------------------sockerUser--------------------')
      console.log('DATA: ', data, 'SOCKET ID:',  socket.id)
      let numberRoom = 'p' + data.numberRoom
      if (data.status === 'player') {
        rooms[numberRoom].players.filter((v, i) => {
          if (v.id.toString() === data.id.toString()) {
            delete rooms[numberRoom].players[i]
          }
        })
        let players = rooms[numberRoom].players.filter(el => {
          return Object.keys(el).length > 0;
        });
        rooms[numberRoom].players = players
        // rooms[numberRoom].players
        let response = {
          id: data.id,
          name: data.name,
          numberPlayer: rooms[numberRoom].numberPlayer - 1,
          type: 'out'
        }
        callback(Constants.Response.Roomstart.Status)
  
        io.to(numberRoom).emit('playerupdate', { status: Constants.Response.Success.Status, data: response })
        io.to(rooms[numberRoom].creator.id).emit('playerupdate', { status: Constants.Response.Roomstart.Status, data: response })
      } else {
        if (rooms[numberRoom].creator.id.toString() === data.id.toString()) {
          delete rooms[numberRoom]
          callback(Constants.Response.Roomstart.Status)
          io.to(numberRoom).emit('bankerOut', Constants.Response.CreatorOut.Status)
        }
      }
    })

    socket.on('disconnect', () => {
      console.log('--------------------disconnect--------------------')
      socket.disconnect()
      // console.log('disconnect ',io.sockets.adapter.rooms);

    })
})

function CheckUsername(playerName) {
  // console.log()
  // if(Object.keys(room).length) { 
    for(let i = 0; i < Object.keys(rooms).length; i++) {
      // if()
      const checkUser =  rooms[Object.keys(rooms)[i]].players.filter(v => playerName.indexOf(v.name) === -1)
      const checkUser2 = rooms[Object.keys(rooms)[i]].creator.name
    }
  // }
}

const RandomChoiceQuestion = () => {
  for (let i = 0; choice.length < 10; i++) {
    let r = Math.floor(Math.random() * 10) + 1
    if (choice.indexOf(r) === -1) {
      choice.push(r)
    }
  }
}

http.listen(port, () => console.log(`listen port ${port}`))


//question
// let _arr = []
// for(let i = 0; i < 100; i++) {
// 	let number = Math.floor(Math.random() * 999) + 1
//   let _number = Math.floor(Math.random() * 999) + 1
//   let key = number > _number ? number : _number
//   let _key = number > _number ? _number : number
//   let index = Math.floor(Math.random() * 4)
//   if (i % 2 === 0) {
//     let _d = [{
//       	id: 'a',
//         text: key+_key
//       }, {
//       	id: 'b',
//         text : key+_key
//       }, {
//       	id: 'c',
//         text: key+_key"
//       }, {
//       	id: 'd',
//         text: key+_key
//       }]
//       let __arr = [1, 10, 11]
//       for (let j = 0; j < _d.length; j++) {
//         if (j !== index) {
//         	var r = __arr[Math.floor(Math.random()*__arr.length)]
//       		let __d = __arr.filter(v=> v !== r)
//           __arr = [...__d]
//           _d[j].text = _d[j].text + r
//         }
//       }
//     _arr = [..._arr, {
//     	question:`${key}+${_key} มีค่าเท่ากับเท่าใด`,
//       choiceList: _d,
//       answer: index === 0 ? 'a' : ( index === 1 ? 'b' : (index === 2 ? 'c' : 'd'))
//     }]
    
//   } else {
//     let _d = [{
//       	id: 'a',
//         text: key-_key
//       }, {
//       	id: 'b',
//         text : key-_key
//       }, {
//       	id: 'c',
//         text: key-_key
//       }, {
//       	id: 'd',
//         text: key-_key
//       }]
//       let __arr = [1, 10, 11]
//       for (let j = 0; j < _d.length; j++) {
//         if (j !== index) {
//         	var r = __arr[Math.floor(Math.random()*__arr.length)]
//       		let __d = __arr.filter(v=> v !== r)
//           __arr = [...__d]
//           _d[j].text = _d[j].text + r
//         }
//       }
//     _arr = [..._arr, {
//     	question:`${key}-${_key} มีค่าเท่ากับเท่าใด`,
//       choiceList: _d,
//       answer: index === 0 ? 'a' : ( index === 1 ? 'b' : (index === 2 ? 'c' : 'd'))
//     }]
//   }
// }
// let _data = {
// 	questionList: _arr
// }
// console.log(_data)
// console.log('_arr ', JSON.stringify(_data))


// let rooms = [{
//   'p60580': {  
//     'creator': {},
//     'players': [],
//     'password': 60580,
//     'maxPlayers': 10,
//     'minPlayers': 2,
//     'numberPlayer': 2,
//     'timer': 0,
//     'isActive': false,
//     'playersAnswerQuestionFirst': [],
//     'skipGame': false
//   }
// }]
  

// let creator = {
//   'id': 'fc3buEjq1FDfcKz-AAAD', 
//   'name': 'Creator name', 
//   'status': 'banker'
// }

// let players = [
//   {  
//     'id': 'aboMOXyRQ3ufYU-dAAAE',
//     'name': 'Player name',
//     'status': 'player',
//     'score': 0,
//     'timeAnswer': 0
//   }
// ]




// { "creator": { "id": "fc3buEjq1FDfcKz-AAAD", "name": "Arm", "status": "banker" } }

// const questionList = [{
//   'question': '39 + 47 มีค่าเท่ากับเท่าใด',
//   'choiceList': [{
//     'id': 'a',
//     'text': 97
//   }, {
//     'id': 'b',
//     'text': 87
//   }, {
//     'id': 'c',
//     'text': 86
//   }, {
//     'id': 'd',
//     'text': 96
//   }],
//   'answer': 'd'
// },{
//   'question': '30 + 24 มีค่าเท่ากับเท่าใด',
//   'choiceList': [{
//     'id': 'a',
//     'text': 65
//   }, {
//     'id': 'b',
//     'text': 54
//   }, {
//     'id': 'c',
//     'text': 64
//   }, {
//     'id': 'd',
//     'text': 55
//   }],
//   'answer': 'd'
// }]
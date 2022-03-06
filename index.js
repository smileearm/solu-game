const app = require('express')()
const http = require('http').Server(app)
const io = require('socket.io')(http)
const Constants = require('./config/constant')
var fs = require('fs');
var datajson = fs.readFileSync('jsTest.json');
var question = JSON.parse(datajson);
app.set('port', process.env.PORT || 3000);
let rooms = {}
let choice = []
let timer = { countdown: {} }
let count = 0
let problems = [0, 1, 2]
let usernamePlayers = []
let countdown = 9
let socketId = []
let player = {}
let name
let timeSkip = 0


io.on('connection', function (socket) {
  console.log('User connected')
  socket.on('checkusernameduplicate', (data, callback) => {
    console.log('data', data)
    if (usernamePlayers.indexOf(data.username) === -1) {
      usernamePlayers.push(data.username)
      callback({ status: Constants.Response.Success.Status, username: data.username })
    } else {
      callback({ status: Constants.Response.UsernameDuplicate.Status, username: '' })
    }
    // if (Object.keys(rooms).length > 0) {
    //   Object.keys(rooms).filter(v => {
    //     if (data.type === 'player') {
    //       for (let i = 0; i < rooms[v].players.length; i++) {
    //         if (rooms[v].players[i].name === data.username) {
    //           console.log('if p')
    //           callback({ status: Constants.Response.UsernameDuplicate.Status, username: '' })
    //         } else {
    //           console.log('else p')
    //           callback({ status: Constants.Response.Success.Status, username: data.username })
    //         }
    //       }
    //     } else { // banker
    //       if (rooms[v].creator.name === data.username) {
    //         console.log('if c')

    //         callback({ status: Constants.Response.UsernameDuplicate.Status, username: '' })
    //       } else {
    //         console.log('else c')

    //         callback({ status: Constants.Response.Success.Status, username: data.username })
    //       }
    //     }
    //   })
    // } else {
    //   // Succsss
    //   callback({ status: Constants.Response.Success.Status, username: data.username })
    // }

  })


  socket.on('checkpassword', (data, callback) => {
    let numberRoom = 'p' + data
    let isCheckRoom = false
    for (let key in rooms) {
      if (key === numberRoom) isCheckRoom = true
    }
    if (isCheckRoom && !rooms[numberRoom].isActive && rooms[numberRoom].maxPlayers > Object.keys(rooms[numberRoom].players).length) {
      callback({ status: Constants.Response.Success.Status })
    } else if (isCheckRoom && !rooms[numberRoom].isActive && rooms[numberRoom].maxPlayers === Object.keys(rooms[numberRoom].players).length) {
      callback({ status: Constants.Response.PlayerMaximum.Status })
    } else if (isCheckRoom && rooms[numberRoom].isActive) {
      callback({ status: Constants.Response.Roomstart.Status })
    } else {
      callback({ status: Constants.Response.InvalidPassword.Status })
    }
  })

  socket.on('createroom', (data, callback) => {
    let numberRoom = 'p' + data.randomPassword
    // allRooms.push(numberRoom)
    if (rooms[numberRoom] === undefined) {
      RandomChoiceQuestion()
      rooms[numberRoom] = {
        creator: {},
        players: [],
        password: data.randomPassword,
        maxPlayers: 9,
        minPlayers: 2,
        numberPlayer: 0,
        timer: 0,
        isActive: false,
        // playersAnswerQuestionFirst: [],
        questionInRoom: choice,
        skipGame: false
      }
    }
    console.log('d', rooms[numberRoom].questionInRoom)
    rooms[numberRoom].creator = {
      id: socket.id,
      name: data.nameUser,
      status: data.status,
    }
    callback({ status: Constants.Response.Success.Status, data: rooms[numberRoom].creator })
  })

  socket.on('joinroom', (data, callback) => {
    let numberRoom = 'p' + data.randomPassword
    rooms[numberRoom].players[socket.id] = {
      id: socket.id,
      name: data.nameUser,
      status: 'player',
      score: 0,
      timeAnswer: 0,
      type: 'in'
    }
    rooms[numberRoom].numberPlayer = Object.keys(rooms[numberRoom].players).length
    CreatePlayersInRoom(numberRoom)
    let response = {
      name: data.nameUser,
      numberPlayer: rooms[numberRoom].numberPlayer,
      id: socket.id,
      type: 'in'
    }
    callback({ status: Constants.Response.Success.Status, data: rooms[numberRoom] })
    io.to(numberRoom).emit('playerupdate', { status: Constants.Response.Success.Status, data: response })
    io.to(rooms[numberRoom].creator.id).emit('playerupdate', { status: Constants.Response.Success.Status, data: response })
    socket.join(numberRoom)
  });

  // socket.on('gamecontroller', (data, callback) => {
  socket.on('gamecontroller', data => {
    let numberRoom = 'p' + data
    socket.join(numberRoom)
    countdown = 9
    rooms[numberRoom].timer = 9

    if (!rooms[numberRoom].skipGame) {
      timer.countdown[numberRoom] = setInterval(() => {
        // timeSkip = rooms[numberRoom].timer--
        io.to(numberRoom).emit('countdownTime', { status: Constants.Response.Success.Status, data: { countdown: rooms[numberRoom].timer-- } })
        if (rooms[numberRoom].timer < 0) {
          clearInterval(timer.countdown[numberRoom])
          delete (timer.countdown[rooms[numberRoom]])
        }
      }, 1000)
    }
    // callback({ status: Constants.Response.Success.Status, data: {countdown: rooms[numberRoom].timer }})
  })

  socket.on('skipGame', data => {
    let numberRoom = 'p' + data
    rooms[numberRoom].timer = 0
    rooms[numberRoom].skipGame = true
    io.to(numberRoom).emit('skipallplayer', {})
  })

  socket.on('question', (data, callback) => {
    console.log('data', data)
    let numberRoom = 'p' + data
    console.log('1', rooms[numberRoom].questionInRoom[count])
    console.log('2', question.questionList[1])

    let response = {
      questionList: question.questionList[rooms[numberRoom].questionInRoom[count]],
      number: problems[count],
      numberProblems: choice[count],
      numberQuestion: count + 1
    }
    console.log('response', response)
    callback({ status: Constants.Response.Success.Status, data: response })
  })

  socket.on('startgame', (data, callback) => {
    let numberRoom = 'p' + data
    rooms[numberRoom].isActive = true
    console.log('startgame', rooms[numberRoom].isActive)
    // if (Object.keys(rooms[numberRoom].players).length >= rooms[numberRoom].minPlayers) {
    io.to(numberRoom).emit('changestartscene', '{}')
    io.to(rooms[numberRoom].creator.id).emit('changestartscene', '{}')
    // callback('Success')
    // } else {
    //     console.log('PLAYER LOWER IN ROOM')
    // }
  })
  socket.on('cancelgame', (data, callback) => {
    let numberRoom = 'p' + data.password
    // socket.join(numberRoom)
    // if (Object.keys(rooms[numberRoom].players).length >= rooms[numberRoom].minPlayers) {
    if (data.status === 'banker') {
      io.to(numberRoom).emit('changecancelscene', '{}')
      io.to(rooms[numberRoom].creator.id).emit('changecancelscene', '{}')
    } else {
      callback('Success')
    }
    // callback('Success')
    // } else {
    //     console.log('PLAYER LOWER IN ROOM')
    // }
  })

  socket.on('playerout', (data, callback) => {
    console.log('data', data)
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
  socket.on('test', (data, callback) => {
    let numberRoom = 'p' + data
    console.log('rooms[numberRoom]', rooms[numberRoom])
    rooms[numberRoom].skipGame = false
    socket.join(numberRoom)
    // rooms[numberRoom].players.filter(x => {
    //     if(x.id.toString() === data.id.toString()) {
    //         x.type = 'out'
    //     }
    // })

    answerQuestionFirst = rooms[numberRoom].players.sort((a, b) => {
      if (a.score === b.score) {
        return b.time - a.time;
      } else {
        return b.score - a.score;
      }
    });
    console.log('answerQuestionFirst', answerQuestionFirst)
    callback({ status: Constants.Response.Success.Status, data: answerQuestionFirst })
  })

  socket.on('checkanswer', (data, callback) => {
    let numberRoom = 'p' + data.numberRoom
    console.log('check', rooms[numberRoom])
    socket.join(numberRoom)
    for (let i = 0; i < (rooms[numberRoom].players).length; i++) {
      if ((rooms[numberRoom].players[i].name === data.name) && (question.questionList[choice[count]].answer === data.id)) {
        rooms[numberRoom].players[i].score += 10 * rooms[numberRoom].timer
        rooms[numberRoom].players[i].timeAnswer += rooms[numberRoom].timer
      }
    }
    io.to(numberRoom).emit('playerAnswer', {})
    callback({ status: Constants.Response.Roomstart.Status, data: rooms[numberRoom] })
  })

  socket.on('getdatauser', (data, callback) => {
    // socketId.push(socket.id)
    let numberRoom = 'p' + data.numberRoom
    console.log('getdata', rooms[numberRoom])
    socket.join(numberRoom)

    let test, response = {}
    rooms[numberRoom].players.filter(v => v.score = 0)
    console.log(rooms[numberRoom])

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
    let numberRoom = 'p' + data
    io.to(numberRoom).emit('changestartscene', '{}')
  })
  socket.on('a123', data => {
    console.log('12312')
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
    socket.user = socket.id
  })
  socket.on('soketJoinroom', data => {
    let numberRoom = 'p' + data
    socket.join(numberRoom)
  })
  socket.on('cancalgame', (data, callback) => {
    let numberRoom = 'p' + data.numberRoom
    io.to(numberRoom).emit('leaveRoom', {})
    socket.leave(numberRoom)
  })
  socket.on('exitroom', (data, callback) => {
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
        name: "arm",
        numberPlayer: rooms[numberRoom].numberPlayer - 1,
        id: data.id,
        type: 'out'
      }
      callback(Constants.Response.Roomstart.Status)

      io.to(numberRoom).emit('playerupdate', { status: Constants.Response.Success.Status, data: response })
      io.to(rooms[numberRoom].creator.id).emit('playerupdate', { status: Constants.Response.Roomstart.Status, data: response })
    } else {
      if (rooms[numberRoom].creator.id.toString() === data.id.toString()) {
        // usernamePlayers.filter(v => v ==='arm');
        callback(Constants.Response.Roomstart.Status)
        io.to(numberRoom).emit('bankerOut', Constants.Response.CreatorOut.Status)
      }
    }
  })
  socket.on('disconnect', () => {
    console.log('disconnectesssd')
  })
})

function RandomChoiceQuestion() {
  for (let i = 0; choice.length < 10; i++) {
    let r = Math.floor(Math.random() * 10) + 1
    if (choice.indexOf(r) === -1) {
      choice.push(r)
    }
  }
}

function CreatePlayersInRoom(numberRoom) {
  let players = rooms[numberRoom].players
  let _players = []
  for (const key in players) {
    _players.push(players[key])
    rooms[numberRoom].players = _players
  }
}

http.listen(3000, () => console.log('listening on 3000'))

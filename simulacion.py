#from pydoc import cram
import random
import agentpy as ap
import socket

s=socket.socket(socket.AF_INET, socket.SOCK_STREAM)
s.connect(("127.0.0.1", 1101))
from_server = s.recv(4096)
print ("Received from server: ",from_server.decode("ascii"))

class Vehicle(ap.Agent):

    def setup(self):
        self.grid = self.model.grid
        self.state = 'straight' # 0 straight, 1 crossing, 2 stopped
        self.dir = 'initial' # up, down, left, right
        self.pos = (0, 0)
        self.counter = 0
        self.nextTurn = random.choice(['straight','left','right'])
        self.id = -1
        self.respawning = 'false'

    def starting_position(self):
        x = 0
        y = 0

        if self.dir == 'up':
            start_pos = random.choice([(12,0),(25,0)])
            x = start_pos[0]
            y = start_pos[1]
            while not (x,y) in self.grid.empty:
                y = y + 1

        if self.dir == 'down':
            start_pos = random.choice([(10,35),(23,35)])
            x = start_pos[0]
            y = start_pos[1]
            while not (x,y) in self.grid.empty:
                y = y - 1

        if self.dir == 'left':
            start_pos = random.choice([(35,12),(35,25)])
            x = start_pos[0]
            y = start_pos[1]
            while not (x,y) in self.grid.empty:
                x = x - 1

        if self.dir == 'right':
            start_pos = random.choice([(0,10),(0,23)])
            x = start_pos[0]
            y = start_pos[1]
            while not (x,y) in self.grid.empty:
                x = x + 1 

        self.grid.move_to(self, (x,y))
        self.pos = (x,y)

    def move(self):

        if not self.state == 'stopped':
            match self.dir:
                case 'up':
                    next_pos = (self.pos[0], self.pos[1] + 1)
                    if next_pos in self.grid.empty:
                        self.grid.move_to(self, next_pos)
                        self.pos = next_pos
                case 'down':
                    next_pos = (self.pos[0], self.pos[1] - 1)
                    if next_pos in self.grid.empty:
                        self.grid.move_to(self, next_pos)
                        self.pos = next_pos
                case 'left':
                    next_pos = (self.pos[0] - 1, self.pos[1])
                    if next_pos in self.grid.empty:
                        self.grid.move_to(self, next_pos)
                        self.pos = next_pos
                case 'right':
                    next_pos = (self.pos[0] + 1, self.pos[1])
                    if next_pos in self.grid.empty:
                        self.grid.move_to(self, next_pos)
                        self.pos = next_pos

    def turn(self):

        match self.dir:
            case 'up':
                if self.nextTurn == 'left':
                    self.dir = 'left'
                elif self.nextTurn == 'right':
                    self.dir = 'right'

            case 'down':
                if self.nextTurn == 'left':
                    self.dir = 'right'
                elif self.nextTurn == 'right':
                    self.dir = 'left'

            case 'left':
                if self.nextTurn == 'left':
                    self.dir = 'down'
                elif self.nextTurn == 'right':
                    self.dir = 'up'

            case 'right':
                if self.nextTurn == 'left':
                    self.dir = 'up'
                elif self.nextTurn == 'right':
                    self.dir = 'down'

        self.nextTurn = random.choice(['straight','left','right'])

class Stoplight(ap.Agent):

    def setup(self):
        self.grid = self.model.grid
        self.dir = 'intial'
        self.color = 'red'

    def lightInit(self, cont):
        match cont:
            case 0:
                self.dir = 'north'
                self.color = 'green'
            case 1:
                self.dir = 'south'
            case 2:
                self.dir = 'west'
            case 3:
                self.dir = 'east'

class City(ap.Model):

    def setup(self):
        self.grid = ap.Grid(self, (36, 36), track_empty=True)
        cars = self.cars = ap.AgentList(self, self.p.car_pop, Vehicle)
        lights = self.lights = ap.AgentList(self, self.p.light_pop, Stoplight)
        self.crossings = []
        self.counter = 0
        self.stepString = ""

        self.grid.add_agents(cars)

        id=0
        carsPos = ""
        for c in cars:
            c.id = id
            id = id+1
            c.dir = random.choice(['up', 'down', 'left', 'right'])
            c.starting_position()
            carsPos = carsPos + str(c.pos[0]) + " " + str(c.pos[1]) + "$"
        print(carsPos)
        s.send(bytes(carsPos,'utf-8'))

        count = 0
        for l in lights:
            l.lightInit(count)
            self.crossings.append(l) # 0: north, 1: south, 2: west, 3: east
            count = count + 1

        
    def step(self):

        for c in self.cars:

            match c.dir:
                case 'up':
                    if c.pos[1] == 35:
                        c.dir = random.choice(['up', 'down', 'left', 'right'])
                        c.starting_position()
                        c.respawning = "true"
                case 'down':
                    if c.pos[1] == 0:
                        c.dir = random.choice(['up', 'down', 'left', 'right'])
                        c.starting_position()
                        c.respawning = "true"
                case 'left':
                    if c.pos[0] == 0:
                        c.dir = random.choice(['up', 'down', 'left', 'right'])
                        c.starting_position()
                        c.respawning = "true"
                case 'right':
                    if c.pos[0] == 35:
                        c.dir = random.choice(['up', 'down', 'left', 'right'])
                        c.starting_position()
                        c.respawning = "true"

            if c.pos == (10,10) or c.pos == (10,23) or c.pos == (23,10) or c.pos == (23,23):
                if c.dir == 'right' and c.nextTurn == 'right':
                    c.turn()
                elif c.dir == 'down' and c.nextTurn == 'left':
                    c.turn()
            
            if c.pos == (10,12) or c.pos == (10,25) or c.pos == (23,12) or c.pos == (23,25):
                if c.dir == 'left' and c.nextTurn == 'left':
                    c.turn()
                elif c.dir == 'down' and c.nextTurn == 'right':
                    c.turn()

            if c.pos == (12,10) or c.pos == (12,23) or c.pos == (25,10) or c.pos == (25,23):
                if c.dir == 'right' and c.turn == 'left':
                    c.turn()
                elif c.dir == 'up' and c.turn == 'right':
                    c.turn()

            if c.pos == (12,12) or c.pos == (12,25) or c.pos == (25,12) or c.pos == (25,25):
                if c.dir == 'left' and c.nextTurn == 'right':
                    c.turn()
                elif c.dir == 'up' and c.nextTurn == 'left':
                    c.turn()


            if c.dir == 'up':
                if c.pos[1] == 9 or c.pos[1] == 22:
                    if self.crossings[0].color != 'green':
                        c.state = 'stopped'
                    else:
                        c.state = 'straight'
            elif c.dir == 'down':
                if c.pos[1] == 13 or c.pos[1] == 26:
                    if self.crossings[1].color != 'green':
                        c.state = 'stopped'
                    else:
                        c.state = 'straight'
            elif c.dir == 'left':
                if c.pos[0] == 13 or c.pos[0] == 26:
                    if self.crossings[2].color != 'green':
                        c.state = 'stopped'
                    else:
                        c.state = 'straight'
            elif c.dir == 'right':
                if c.pos[0] == 9 or c.pos[0] == 22:
                    if self.crossings[3].color != 'green':
                        c.state = 'stopped'
                    else:
                        c.state = 'straight'

            c.move()
            self.stepString = self.stepString + str(c.id) + " " + str(c.pos[0]) + " " + str(c.pos[1]) + " " + str(c.dir) + " " + c.respawning + "$"
            c.respawning = "false"

        self.stepString = self.stepString + "%"
        for l in self.lights:
            self.stepString = self.stepString + str(l.dir) + " " + str(l.color) + "$"

        if self.counter == 10:
            for i in range(len(self.crossings)):
                if self.crossings[i].color == 'green':
                    self.crossings[i].color = 'yellow'

        elif self.counter == 15:
            for i in range(len(self.crossings)):
                if self.crossings[i].color == 'yellow':
                    self.crossings[i].color = 'red'
                    if i == len(self.crossings) - 1:
                        self.crossings[0].color = 'green'
                    else:
                        self.crossings[i+1].color = 'green'
            self.counter = 0
        

        self.counter = self.counter + 1

        #print(self.stepString)
        s.send(bytes(self.stepString,'utf-8'))
        self.stepString=""
        from_server = s.recv(4096)
        #print ("Received from server: ",from_server.decode("ascii"))
 
parameters = {
    'car_pop': 20,
    'light_pop': 4,
    'steps': 200,
}

bcar_pop = bytes(str(parameters['car_pop']),'utf-8')
s.send(bcar_pop)

model = City(parameters)
results = model.run()
s.send(b"<EOF>")

# Docker for Golbaus

### Deploy Golbaus project with docker (Nginx, ASP.NET, React, MySQL)

Clone project and create a directory structure as described below (default.conf and docker-compose.yml in folder `Docker` in Golbaus_BE):

```
├── default.conf
├── docker-compose.yml
├── Golbaus_BE
├── Golbaus_FE
```

Go to folder Golbaus_FE and build project

```bash
$ cd Golbaus_FE
$ npm install
$ npm run build
```

Now go to parent folder and run docker

```bash
$ cd ..
$ docker compose -p golbaus up -d
```

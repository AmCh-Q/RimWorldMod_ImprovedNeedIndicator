
## Requirements

There are two options. You can either install the requirements onto your local machine or use the containerized version included in this repository.

### Containerized

If you use the containerized version then all you need to have installed is `Podman` or `Docker`. The included scripts assume `Podman`, but you can adapt them to docker easily by swapping out the `podman` command for `docker`.

#### Setup

Setup the container

```shell
bash Scripts/setup.sh
```

#### Build

Builds the solution, placing each assembly into the appropriate versioned folder

```shell
bash Scripts/build.sh
```

#### Deploy

Copies files from this repo into the folder defined in the `RIMWORLD_MOD_PATH` environment variable set in `.env`.

```shell
bash Scripts/deploy.sh
```

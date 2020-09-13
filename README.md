# .net core를 통한 간단한 Serilog 및 Seq 사용

## seq dockerize

```bash
$ docker volume create seq_data
$ docker run --name seq -d --restart unless-stopped -e ACCEPT_EULA=Y -v seq_data:/data -p 5340:80 -p 5341:5341 datalust/seq:latest
```

## LICENSE
MIT

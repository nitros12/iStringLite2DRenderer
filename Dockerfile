FROM mono
ADD . /src
WORKDIR /src
RUN msbuild /p:Configuration=Release
CMD [ "mono", "/src/iStringLite2DRenderer/bin/Release/iStringLite2DRenderer.exe" ]

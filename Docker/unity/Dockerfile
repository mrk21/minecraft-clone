FROM gableroux/unity3d:2019.3.0f6-webgl

# Install basic packages
RUN apt-get update -qq && \
    apt-get install -y build-essential tree vim less curl dnsutils net-tools procps && \
    apt-get install -y --no-install-recommends apt-utils

# Install Ruby
RUN apt-get install -y ruby

# Install ImageMagick
RUN apt-get install -y imagemagick

# Install openupm
RUN npm install -g openupm-cli

# Change default shell to bash from dash
RUN echo "dash dash/sh boolean false" | debconf-set-selections && \
    dpkg-reconfigure dash

# Set app dir
ENV APP_HOME /app
ENV SHELL /bin/bash
ENV EDITOR vim
ENV UNITY_CMD /opt/Unity/Editor/Unity
ENV PATH $APP_HOME/bin:$PATH
RUN mkdir -p $APP_HOME

WORKDIR $APP_HOME

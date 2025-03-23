#------------------------------------------------------------------------------
# Entrykit 0.4.1
# Entrykit 0.4.1 hasn't been released yet, so we build from repository
#------------------------------------------------------------------------------
FROM golang AS build_entrykit
WORKDIR /
RUN git clone https://github.com/progrium/entrykit.git && \
    cd entrykit && \
    git checkout 799207016380fc3d22e2d1f76894a7ce5c98613c && \
    make

#------------------------------------------------------------------------------
# Base
#------------------------------------------------------------------------------
FROM unityci/editor:ubuntu-6000.0.37f1-webgl-3.1

# Install basic packages
RUN apt update -qq && \
    apt install -y build-essential tree vim less curl unzip dnsutils net-tools procps && \
    apt install -y --no-install-recommends apt-utils && \
    apt install -y gettext

# Install Ruby
RUN apt install -y ruby

# Install Node.js(System)
RUN apt install -y nodejs npm

# Install Node.js 22.14.0
# @see https://www.trifields.jp/how-to-install-node-js-on-ubuntu1804-3544
RUN npm install -g n && n 22.14.0

# Install ImageMagick
RUN apt install -y imagemagick

# Install openupm
RUN npm install -g openupm-cli

# Install aws-cli
RUN curl "https://d1vvhvl2y92vvt.cloudfront.net/awscli-exe-linux-x86_64.zip" -o "awscliv2.zip" && \
    unzip awscliv2.zip && \
    ./aws/install && \
    rm -rf awscliv2.zip && \
    rm -rf aws

# Install firebase-tools
RUN npm install -g firebase-tools

# Install Entrykit
COPY --from=build_entrykit /entrykit/build/Linux/entrykit /bin/entrykit
RUN entrykit --symlink

# Change default shell to bash from dash
RUN echo "dash dash/sh boolean false" | debconf-set-selections && \
    dpkg-reconfigure dash

# Set app dir
ENV APP_HOME /app
ENV SHELL /bin/bash
ENV EDITOR vim
ENV UNITY_CMD /opt/unity/Editor/Unity
ENV ULF_FILE Unity_v6000.x.ulf
ENV PATH $APP_HOME/bin:$PATH
RUN mkdir -p $APP_HOME

WORKDIR $APP_HOME
ENTRYPOINT [ "prehook", "/app/bin/make_firebaserc", "--" ]

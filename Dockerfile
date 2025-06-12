FROM nvcr.io/nvidia/pytorch:25.05-py3-igpu

# 기본 패키지 설치
RUN apt update && apt install -y \
    git curl wget unzip ffmpeg \
    libgl1-mesa-glx libglib2.0-0 libsm6 libxrender1 libxext6 \
    libgtk2.0-dev pkg-config

# OpenCV 설치 (pip 버전, GPU 가속 없이 최소 구성)
RUN pip install --upgrade pip && \
    pip install opencv-python-headless

# YOLOv8 및 기타 의존성 설치
RUN pip install ultralytics matplotlib

# 디폴트 작업 디렉토리
WORKDIR /workspace

# 포트 및 장치 노출 (웹캠 등)
ENV DEBIAN_FRONTEND=noninteractive
ENV DISPLAY=:0

CMD ["/bin/bash"]

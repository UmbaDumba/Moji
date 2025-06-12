from ultralytics import YOLO

# 학습 설정
model = YOLO("yolov8n.pt")  # 경량 모델 (Nano용), 처음 학습할 때 사용

# 데이터셋 구성 파일 (.yaml 경로)
data_yaml = "/workspace/Moji/image_eye/data.yaml"

# 학습 실행
model.train(
    data=data_yaml,
    epochs=30,
    imgsz=640,
    batch=16,
    name="eye_tracking",  # runs/detect/eye_tracking/weights/best.pt 로 저장됨
    device=0              # GPU 사용 (Jetson Orin에서는 device=0)
)

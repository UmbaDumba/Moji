from ultralytics import YOLO

# YOLOv8 모델 불러오기
model = YOLO("yolov8n.pt")  # 다른 모델: yolov8s.pt, yolov8m.pt 등

# 학습 실행
model.train(
    data="woo_dataset/data.yaml",  # 데이터셋 경로
    epochs=50,
    imgsz=640,
    batch=8,
    project="runs",
    name="eye_yolo_train"
)

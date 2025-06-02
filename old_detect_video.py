from ultralytics import YOLO
import cv2

# 1단계: 사람 탐지용 YOLO 모델 (사전 학습된 COCO)
person_detector = YOLO("yolov8n.pt")  # 또는 yolov8s.pt 등

# 2단계: 학습된 눈/눈동자 모델
eye_model = YOLO("runs/eye_yolo_train/weights/best.pt")

# 웹캠 열기
cap = cv2.VideoCapture(0)  # 1 또는 0은 캠 번호

while True:
    ret, frame = cap.read()
    if not ret:
        break

    # 1단계: 사람 검출
    person_results = person_detector(frame)
    person_boxes = person_results[0].boxes

    for box in person_boxes:
        cls_id = int(box.cls[0].item())
        if cls_id != 0:  # 0: person 클래스 (YOLO COCO 기준)
            continue

        # 박스 좌표 추출
        x1, y1, x2, y2 = map(int, box.xyxy[0])
        person_crop = frame[y1:y2, x1:x2]

        # 2단계: 눈/눈동자 모델로 감지
        eye_results = eye_model(person_crop)
        eye_annotated = eye_results[0].plot()

        # 원본 프레임에 결과 붙여넣기
        frame[y1:y2, x1:x2] = eye_annotated

    # 결과 출력
    cv2.imshow("YOLOv8 Person + Eye Detection", frame)

    if cv2.waitKey(1) & 0xFF == ord('q'):
        break

cap.release()
cv2.destroyAllWindows()

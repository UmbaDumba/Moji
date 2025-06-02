from ultralytics import YOLO
import cv2
from deepface import DeepFace

# YOLO 모델들
person_detector = YOLO("yolov8s.pt")
eye_model = YOLO("runs/eye_yolo_train/weights/best.pt")

# 홍길동 벡터 준비
hong_embedding = DeepFace.represent(img_path=r"C:\Users\user\Moji\test\images\u7_jpg.rf.e313c4ab387ad6ee0ca70414619e6c9c.jpg", model_name="Facenet")[0]["embedding"]

# 웹캠
cap = cv2.VideoCapture(2)

while True:
    ret, frame = cap.read()
    if not ret:
        break

    # 1단계: 사람 탐지
    person_results = person_detector(frame)
    person_boxes = person_results[0].boxes

    for box in person_boxes:
        cls_id = int(box.cls[0].item())
        if cls_id != 0:  # person 클래스
            continue

        # 사람 crop
        x1, y1, x2, y2 = map(int, box.xyxy[0])
        person_crop = frame[y1:y2, x1:x2]
        try:
            # 2단계: 얼굴 유사도 비교
            result = DeepFace.verify(person_crop, "./woo_dataset/test/images/u7_jpg.rf.e313c4ab387ad6ee0ca70414619e6c9c.jpg", model_name="Facenet", enforce_detection=False)

            if result["verified"] and result["distance"] < 0.4:  # threshold 조정 가능
                # WOO라면 눈 탐지
                eye_results = eye_model(person_crop)
                eye_annotated = eye_results[0].plot()

                # 결과 덮어쓰기
                frame[y1:y2, x1:x2] = eye_annotated

                # 이름 표시
                cv2.putText(frame, "WOO", (x1, y1 - 10), cv2.FONT_HERSHEY_SIMPLEX, 0.8, (0,255,0), 2)

        except Exception as e:
            print("Error Recognition:", e)
            continue

    # 출력
    cv2.imshow("WOO trakcing + eye detecting", frame)
    if cv2.waitKey(1) & 0xFF == ord('q'):
        break

cap.release()
cv2.destroyAllWindows()
